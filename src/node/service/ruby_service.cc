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

#include "node/zeromq/context.h"
#include "node/zeromq/socket.h"
#include "node/zeromq/message.h"
#include "node/service/constants.h"

namespace node {

RubyService::RubyService(zmq::Context* context)
  : ServiceBase(node::kRubyServiceName),
    context_(context),
    port_(node::kRequestReplyPort),
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
  std::string endpoint("tcp://*:");
  endpoint.append(base::IntToString(port_));

  zmq::Socket socket(context_->CreateSocket(zmq::kReply));
  if (socket.Bind(endpoint.c_str())) {
    while (is_running_) {
      zmq::Message* message = socket.Receive(0).get();
      if (message->size() > 0) {
        LOG(INFO) << message->data();
      }
    }
  }
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