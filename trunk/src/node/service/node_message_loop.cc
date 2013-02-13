// Copyright (c) 2010 Nohros Systems Inc. All rights reserved
// Use of this source code is governed by BSD-style license that can be found
// in the LICENCE file.

#include <vector>

#include "node/service/node_message_loop.h"

#include <base/logging.h>
#include <base/string_number_conversions.h>
#include <google/protobuf/repeated_field.h>

#include <ruby_protos.pb.h>
#include <control.pb.h>
#include "node/zeromq/context.h"
#include "node/zeromq/socket.h"
#include "node/zeromq/message.h"
#include "node/service/constants.h"
#include "node/service/message_router.h"
#include "node/service//zero_copy_message.h"

namespace node {

const char* NodeMessageLoop::kInvalidMessage = 
  "Invalid message format.";

namespace rpc = ::ruby::protocol::control;
namespace rp = ::ruby::protocol;
namespace gpb = google::protobuf;

class ServicesDatabase;

NodeMessageLoop::NodeMessageLoop(zmq::Context* context,
  MessageRouter* message_router)
  : context_(context),
    message_router_(message_router),
    message_channel_port_(node::kMessageChannelPort),
    run_called_(false),
    quit_called_(false),
    running_(false) {
  DCHECK(context);
  DCHECK(message_router);
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
  if (dealer_.get()) {
    dealer_->Close();
  }
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
    rp::RubyMessagePacket packet;
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
  rp::RubyMessagePacket packet;
  rp::RubyMessage* msg = packet.mutable_message();
  msg->set_id(0);

  int zmq_message_size = packet.ByteSize();
  scoped_refptr<ZeroCopyMessage> zero_copy_message(
    new ZeroCopyMessage(zmq_message_size));
  packet.SerializeToZeroCopyStream(zero_copy_message);

  //  Packet pattern should be [EMPTY FRAME] [DATA]
  if (!dealer_->Send(zmq::kSendMore) ||
    !dealer_->Send(zero_copy_message, zmq_message_size, zmq::kNoFlags)) {
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

void NodeMessageLoop::ProcessMessage(const rp::RubyMessage& ruby_message) {
  if (!ruby_message.has_message()) {
    ReportError(RUBY_CONTROL_INVALID_MESSAGE);
    return;
  }

  std::string message = ruby_message.message();
  switch(ruby_message.type()) {
    case rpc::kServiceControl:
      break;

    case rpc::kNodeAnnounce:
      Announce(message);
      break;

    case rpc::kNodeQuery:
      QueryService(message);
      break;
  }
}

void NodeMessageLoop::Announce(const std::string& message) {
  rpc::AnnounceMessage announce_message;
  if (!announce_message.ParseFromString(message)) {
    ReportError(RUBY_CONTROL_INVALID_MESSAGE);
    return;
  }

  ServiceFactSet facts;
  announce_message.facts();
  for (gpb::
}

void NodeMessageLoop::QueryService(const std::string& message) {
}

void NodeMessageLoop::ReportError(ProcessingError error_code) {
  ruby::ExceptionMessage exception;
  exception.set_code(error_code);
  exception.set_message(ErrorCodeToString(error_code));
  exception.set_source(kNodeServiceName);
}

std::string NodeMessageLoop::ErrorCodeToString(ProcessingError error_code) {
  switch(error_code) {
    case RUBY_CONTROL_NO_ERROR:
      return std::string();

    case RUBY_CONTROL_INVALID_MESSAGE:
      return kInvalidMessage;
  }
}

}  // namespace node
