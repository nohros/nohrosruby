// Copyright (c) 2010 Nohros Systems Inc. All rights reserved
// Use of this source code is governed by BSD-style license that can be found
// in the LICENCE file.

#include <winsock2.h>
#include <windows.h>
#include <vector>

#include "node/service/ruby_service.h"

#include <base/basictypes.h>
#include <base/path_service.h>
#include <base/base_paths.h>
#include <base/file_path.h>
#include <base/command_line.h>
#include <base/logging.h>
#include <base/memory/ref_counted.h>
#include <base/string_number_conversions.h>
#include <base/string_util.h>
#include <base/threading/platform_thread.h>
#include <sql/connection.h>

#include "node/zeromq/socket.h"
#include "node/zeromq/context.h"
#include "node/zeromq/diagnostic_error_delegate.h"
#include "node/service/constants.h"
#include "node/service/ruby_switches.h"
#include "node/service/message_router.h"
#include "node/service/message_receiver.h"
#include "node/service/node_message_loop.h"
#include "node/service/constants.h"
#include "node/service/services_database.h"
#include "node/service/routing_database.h"

namespace node {

RubyService::RubyService()
  : ServiceBase(node::kRubyServiceName) {
}

bool RubyService::OnStart(const std::vector<std::wstring>& arguments) {
  LOG(INFO) << "Service has been started";

  const CommandLine& switches = *CommandLine::ForCurrentProcess();

  // Override the default message channel port.
  if (switches.HasSwitch(switches::kMessageChannelPort)) {
    std::string value =
      switches.GetSwitchValueASCII(switches::kMessageChannelPort);

    int message_channel_port;
    if (!base::StringToInt(value, &message_channel_port)) {
      message_channel_port = node::kMessageChannelPort;
    } else {
      LOG(WARNING) << "Failed to parse the request reply port: "
              << value
              << ". Using the default port: "
              << node::kMessageChannelPort;
    }
  }

  // Set up the service tracker address, if supplied.
  // TODO(neylor.silva) Tracker address code.
  if (switches.HasSwitch(switches::kServiceTrackerAddress)) {
  }

  // Set up the services databases.
  FilePath services_database_path;
  if (!PathService::Get(base::FILE_EXE, &services_database_path)) {
    NOTREACHED();
    return false;
  }
  services_database_path = services_database_path
    .DirName()
    .Append(node::kServicesDatabaseFilename);

  services_db_.reset(new ServicesDatabase());
  if (!services_db_->Open(services_database_path)) {
    LOG(ERROR) << "Unable to open services database.";
    return false;
  }

  routing_db_.reset(new RoutingDatabase());
  if (!routing_db_->Open()) {
    LOG(ERROR) << "Unable to open the routing database.";
    return false;
  }

  // Setup the zmq Context.
  context_.reset(new zmq::Context());
  context_->set_error_delegate(new zmq::DiagnosticErrorDelegate());
  if (!context_->Open(1)) {
    NOTREACHED() << "zmq::Context failed to open.";
    return false;
  }

  message_router_.reset(
    new MessageRouter(services_db_.get(), routing_db_.get()));
  message_receiver_.reset(
    new MessageReceiver(context_.get(), message_router_.get()));
  message_loop_.reset(
    new MessageLoop(context_.get(), message_router_.get(), services_db_.get()));

  service_thread_delegate_.reset(new ServiceThreadDelegate(this));
  if (!base::PlatformThread::Create(
    0, service_thread_delegate_.get(), &service_thread_)) {
    NOTREACHED() << "Control message thread creation failed.";
  }

  return true;
}

RubyService::ServiceThreadDelegate::ServiceThreadDelegate(RubyService* service)
  : service_(service) {
  DCHECK(service);
}

void RubyService::ServiceThreadDelegate::ThreadMain() {
  service_->Run();
}

void RubyService::Run() {
  MessageLoopThreadDelegate message_loop_thread_delegate(message_loop_.get());
  base::PlatformThreadHandle message_loop_thread;

  // Runs the message loop in a dedicated thread.
  if (!base::PlatformThread::Create(
    0, &message_loop_thread_delegate, &message_loop_thread)) {
    NOTREACHED() << "Message loop message thread creation failed.";
  }

  // Start receiving messages
  message_receiver_->Run();

  // Wait the message loop thread to finish.
  base::PlatformThread::Join(message_loop_thread);
}

RubyService::MessageLoopThreadDelegate::MessageLoopThreadDelegate(
  MessageLoop* message_loop)
  : message_loop_(message_loop) {
}

// PlatformThread::Delegate() implementation
void RubyService::MessageLoopThreadDelegate::ThreadMain() {
  message_loop_->Run();
}

void RubyService::OnStop() {
  message_loop_->Quit();
  message_receiver_->Stop();
  context_->Close();

  if (service_thread_) {
    base::PlatformThread::Join(service_thread_);
  }
}

RubyService::~RubyService() {
}

}  // namespace node