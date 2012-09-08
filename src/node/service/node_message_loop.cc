// Copyright (c) 2010 Nohros Systems Inc. All rights reserved
// Use of this source code is governed by BSD-style license that can be found
// in the LICENCE file.

#include <vector>

#include "node/service/node_message_loop.h"

#include <base/logging.h>
#include <base/string_number_conversions.h>

#include <ruby_protos.pb.h>
#include <control.pb.h>
#include "node/zeromq/context.h"
#include "node/zeromq/socket.h"
#include "node/zeromq/message.h"
#include "node/service/constants.h"
#include "node/service/message_router.h"

namespace node {
class ServicesDatabase;

NodeMessageLoop::NodeMessageLoop(zmq::Context* context,
  MessageRouter* message_router)
  : context_(context),
    message_router_(message_router),
    message_channel_port_(node::kMessageChannelPort),
    run_called_(false),
    quit_called_(false),
    running_(false) {
  //DCHECK(context);
}

NodeMessageLoop::~NodeMessageLoop() {
}

void NodeMessageLoop::Run() {
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
    // register ourself into the routing database.
    if (!RegisterRoute()) {
      LOG(ERROR) << "The node message receiver cannot be registered.";
      return;
    }

    // Loop for control messages
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

void NodeMessageLoop::Quit() {
  quit_called_ = true;
}

void NodeMessageLoop::OnMessageReceived(const MessageParts& message_parts) {
  int no_of_parts = message_parts.size();
  if (no_of_parts % 3 != 0) {
    LOG (WARNING) << "Received message has a invalid number of parts."
              << "No of parts: " << no_of_parts;
    return;
  }

  // The received message envelope pattern should be: [EMPTY FRAME] [DATA]
  for (MessageParts::const_iterator i = message_parts.begin() + 1;
    i != message_parts.end(); i += 2) {
    zmq::Message* message = i->get();
    protocol::RubyMessagePacket packet;
    if (!packet.ParseFromArray(message->mutable_data(), message->size())) {
      LOG(WARNING) << "The received message is not a valid ruby message packet.";
      continue;
    }

    if (!packet.has_message()) {
      LOG(WARNING) << "Received a packet with no message associated.";
      continue;
    }
  }
}

bool NodeMessageLoop::RegisterRoute() {
  // We need to send a message to the message receiver to discover our
  // routing address.
  protocol::RubyMessagePacket packet;
  packet.mutable_message();

  int zmq_message_size = packet.ByteSize();
  scoped_refptr<zmq::Message> zmq_message(new zmq::Message(zmq_message_size));

  //  Packet pattern should be [EMPTY FRAME] [DATA]
  if (!dealer_->Send(zmq::kSendMore) ||
    !dealer_->Send(zmq_message, zmq_message_size, zmq::kNoFlags)) {
    return false;
  }

  MessageParts parts;
  if (!dealer_->Receive(&parts, zmq::kNoFlags)) {
    return false;
  }

  DCHECK(parts.size() == 2);

  // Get the address from the reply and store it on routing database.
  packet.Clear();
  scoped_refptr<zmq::Message> message = parts[1];
  if (!packet.ParseFromArray(message->mutable_data(), message->size())) {
    return false;
  }

  DCHECK(packet.has_message());

  // Route all message sent to the service named [kNodeServiceName] to the
  // node message loop.
  ServiceFactSet facts;
  facts.push_back(std::make_pair(kServiceNameFact, kNodeServiceName));

  return
    message_router_->AddRoute(packet.message().sender(), facts);
}

/*void NodeMessageLoop::ProcessMessage(const protocol::RubyMessage& message) {
  switch(message.type()) {
    case protocol::control::kServiceControlStart:
      break;

    case protocol::control::kServiceControlStop:
      break;

    case protocol::control::kServiceControlPause:
      break;

    case protocol::control::kServiceControlContinue:
      break;
  }
}*/

}  // namespace node
