// Copyright (c) 2010 Nohros Systems Inc. All rights reserved
// Use of this source code is governed by BSD-style license that can be found
// in the LICENCE file.

#include <vector>

#include "node/service/control_message_loop.h"

#include <base/logging.h>
#include <base/string_number_conversions.h>

#include <ruby_protos.pb.h>
#include "node/zeromq/context.h"
#include "node/zeromq/socket.h"
#include "node/zeromq/message.h"
#include "node/service/constants.h"
#include "node/service/message_router.h"

namespace node {
class ServicesDatabase;

ControlMessageLoop::ControlMessageLoop(zmq::Context* context,
  MessageRouter* message_router)
  : context_(context),
    message_router_(message_router),
    message_channel_port_(node::kMessageChannelPort),
    run_called_(false),
    quit_called_(false),
    running_(false) {
}

ControlMessageLoop::~ControlMessageLoop() {
}

void ControlMessageLoop::Run() {
  DCHECK(!run_called_);
  run_called_ = true;

  std::string endpoint("tcp://127.0.0.1:");
  endpoint.append(base::IntToString(message_channel_port_));

  // Allow Quit to be called before Run.
  if (quit_called_) {
    return;
  }

  dealer_.reset(new zmq::Socket(context_->CreateSocket(zmq::kDealer)));
  if (dealer_.get() && dealer_->Connect(endpoint.c_str())) {
    MessageParts parts;
    while (!quit_called_ && !context_->is_terminating()) {
      if (dealer_->Receive(&parts, zmq::kNoFlags)) {
        OnMessageReceived(parts);
      }
      parts.clear();
    }
  }

  running_ = false;
}

void ControlMessageLoop::Quit() {
  quit_called_ = true;
}

void ControlMessageLoop::OnMessageReceived(const MessageParts& message_parts) {
}

}  // namespace node
