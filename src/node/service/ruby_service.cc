// Copyright (c) 2010 Nohros Systems Inc. All rights reserved
// Use of this source code is governed by BSD-style license that can be found
// in the LICENCE file.

#include <winsock2.h>
#include <windows.h>
#include <vector>

#include "node/service/ruby_service.h"

#include <base/file_path.h>
#include <base/command_line.h>
#include <base/logging.h>
#include <base/memory/ref_counted.h>
#include <base/string_number_conversions.h>
#include <base/string_util.h>

#include "node/zeromq/context.h"
#include "node/zeromq/diagnostic_error_delegate.h"
#include "node/service/constants.h"
#include "node/service/ruby_switches.h"
#include "node/service/message_router.h"
#include "node/service/message_receiver.h"
#include "node/service/node_message_loop.h"

namespace node {
namespace {

zmq::ErrorDelegate* GetErrorHandler() {
  return new zmq::DiagnosticErrorDelegate();
}

}  // anonymous namespace

RubyService::RubyService(MessageRouter* router)
  : ServiceBase(node::kRubyServiceName),
    context_(new zmq::Context()),
    message_channel_port_(node::kMessageChannelPort),
    message_receiver_(new MessageReceiver(NULL, router)),
    node_message_loop_(new NodeMessageLoop(context_.get(), router)),
    node_message_loop_delegate_(
      new NodeMessageLoopThreadDelegate(node_message_loop_.get())) {
  DCHECK(!context_.get());
  DCHECK(!node_message_loop_.get());
  DCHECK(!node_message_loop_delegate_.get());
}

void RubyService::OnStart(const std::vector<std::wstring>& arguments) {
  LOG(INFO) << "Service has been started";

  const CommandLine& switches = *CommandLine::ForCurrentProcess();
  context_->set_error_delegate(GetErrorHandler());
  // Override the default message channel port.
  if (switches.HasSwitch(switches::kMessageChannelPort)) {
    std::string value =
      switches.GetSwitchValueASCII(switches::kMessageChannelPort);

    int message_channel_port;
    if (base::StringToInt(value, &message_channel_port)) {
      message_channel_port_ = message_channel_port;
    } else {
      LOG(WARNING) << "Failed to parse the request reply port: "
              << value
              << ". Using the default port: "
              << node::kMessageChannelPort;
    }
  }

  // Set up the service tracker address, if supplied.
  if (switches.HasSwitch(switches::kServiceTrackerAddress)) {
  }

  context_->set_error_delegate(GetErrorHandler());
  if (!context_->Open(1)) {
    LOG(ERROR) << "Context could not be opened. Error: "
               << context_->GetErrorMessage();
    return;
  }

  /*node_message_loop_delegate_.reset(
    new NodeMessageLoopThreadDelegate(node_message_loop_.get()));
  if (!base::PlatformThread::Create(
    0, node_message_loop_delegate_.get(), &control_message_thread_)) {
    NOTREACHED() << "Control message thread creation failed.";
  }*/

  message_receiver_->Start();
}

RubyService::NodeMessageLoopThreadDelegate::NodeMessageLoopThreadDelegate(
  NodeMessageLoop* node_message_loop)
  : node_message_loop_(node_message_loop) {
}

// PlatformThread::Delegate() implementation
void RubyService::NodeMessageLoopThreadDelegate::ThreadMain() {
  node_message_loop_->Run();
}

void RubyService::OnStop() {
  node_message_loop_->Quit();
  message_receiver_->Stop();
  context_->Close();

  if (control_message_thread_) {
    base::PlatformThread::Join(control_message_thread_);
  }
}

RubyService::~RubyService() {
}

}  // namespace node