// Copyright (c) 2010 Nohros Systems Inc. All rights reserved
// Use of this source code is governed by BSD-style license that can be found
// in the LICENCE file.

#include <winsock2.h>
#include <windows.h>

#include "node/service/ruby_service.h"

#include <base/file_path.h>
#include <base/command_line.h>
#include <base/logging.h>
#include <base/memory/ref_counted.h>
#include <base/threading/platform_thread.h>
#include <base/string_number_conversions.h>
#include <base/memory/scoped_ptr.h>

#include "node/zeromq/context.h"
#include "node/zeromq/socket.h"
#include "node/zeromq/message.h"
#include "node/service/constants.h"

namespace node {

RubyService::RubyService(zmq::Context* context)
  : ServiceBase(node::kRubyServiceName),
    context_(context),
    message_channel_port_(node::kIPCChannelPort),
    thread_(NULL),
    is_running_(false) {
}

void RubyService::OnStart(const std::vector<std::string>& arguments) {
  VLOG(1) << "Service has been started";

  is_running_ = true ;

  if (!base::PlatformThread::Create(0, this, &thread_)) {
    NOTREACHED() << "Service worker thread creation failed.";
  }
}

// PlatformThread::Delegate() implementation
void RubyService::ThreadMain() {
  // create out frontend and backend socket to receive commands from clients
  // and services.
  scoped_ptr<zmq::Socket> router(CreateSocket(message_channel_port_));
  if (router.get()) {
    while (is_running_) {
      zmq::Message* message = router.get()->Receive(zmq::kNoFlags);
      if (message->size() > 0) {
        LOG(INFO) << message->data();
      }
    }
  }
}

zmq::Socket* RubyService::CreateSocket(int port) {
  std::string endpoint("tcp://*");
  endpoint.append(base::IntToString(port));
  scoped_ptr<zmq::Socket> router (new zmq::Socket(
    context_->CreateSocket(zmq::kRouter)));
  if (router.get()->Bind(endpoint.c_str())) {
    return router.release();
  }
  return NULL;
}

void RubyService::OnStop() {
  is_running_ = false;
  context_->Close();

  if (thread_) {
    base::PlatformThread::Join(thread_);
  }
}

RubyService::~RubyService() {
}

}  // namespace node