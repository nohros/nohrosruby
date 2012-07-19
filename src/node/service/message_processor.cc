// Copyright (c) 2010 Nohros Systems Inc. All rights reserved
// Use of this source code is governed by BSD-style license that can be found
// in the LICENCE file.

#include <vector>

#include "node/service/message_processor.h"

#include <base/logging.h>
#include <base/string_number_conversions.h>

#include <ruby_protos.pb.h>
#include "node/zeromq/socket.h"
#include "node/zeromq/message.h"
#include "node/service/constants.h"

namespace node {

MessageProcessor::MessageProcessor(zmq::Context* context)
  : context_(context),
    message_channel_port_(node::kMessageChannelPort) {
}

MessageProcessor::~MessageProcessor() {
}

// PlatformThread::Delegate() implementation
void MessageProcessor::ThreadMain() {
  std::string endpoint("tcp://*:");
  endpoint.append(base::IntToString(message_channel_port_));

  // Create our router socket to receive commands from clients and services.
  dealer_.reset(new zmq::Socket(context_->CreateSocket(zmq::kRouter)));
  if (dealer_.get() && dealer_->Bind(endpoint.c_str())) {
    zmq::MessageParts parts;
    while (!context_->is_terminating()) {
      if (dealer_->Receive(&parts, zmq::kNoFlags)) {
      }
      parts.clear();
    }
    return;
  }
}

int MessageProcessor::OnError(int error, zmq::Context* context, zmq::Socket* socket) {
  LOG(ERROR) << "zmq error " << error
             << ", errno " << context->GetErrorCode()
             << ": " << context->GetErrorMessage();
  return error;
}

}  // namespace node
