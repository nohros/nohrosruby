// Copyright (c) 2010 Nohros Systems Inc. All rights reserved
// Use of this source code is governed by BSD-style license that can be found
// in the LICENCE file.

#include "node/service/message_receiver.h"

#include <base/logging.h>
#include <base/string_number_conversions.h>
#include <google/protobuf/repeated_field.h>
#include <ruby_protos.pb.h>

#include "node/zeromq/context.h"
#include "node/zeromq/socket.h"
#include "node/zeromq/message.h"
#include "node/service/constants.h"
#include "node/service/message_router.h"

namespace node {

MessageReceiver::MessageReceiver(zmq::Context* context, MessageRouter* router)
  : context_(context),
    router_(router),
    message_channel_port_(node::kMessageChannelPort),
    running_(false) {
  DCHECK(context);
  DCHECK(router);
}

MessageReceiver::~MessageReceiver() {
}

void MessageReceiver::Start() {
  std::string endpoint("tcp://*:");
  endpoint.append(base::IntToString(message_channel_port_));

  scoped_ptr<zmq::Socket> router;
    //new zmq::Socket(context_->CreateSocket(zmq::kRouter)));
  if (router.get() && router->Bind(endpoint.c_str())) {
    MessageParts parts;
    while (!running_ && !context_->is_terminating()) {
      if (router->Receive(&parts, zmq::kNoFlags)) {
        OnMessageReceived(router.get(), parts);
      }
      parts.clear();
    }
  }
}

void MessageReceiver::Stop() {
  running_ = false;
}

void MessageReceiver::OnMessageReceived(zmq::Socket* socket,
  const MessageParts& message_parts) {
  int no_of_parts = message_parts.size();
  if (no_of_parts % 3 != 0) {
    LOG (WARNING) << "Received message has a invalid number of parts."
              << "No of parts: " << no_of_parts;
    return;
  }

  // The message format is:
  //   [sender id][empty frame][message]
  scoped_refptr<zmq::Message> message = message_parts[2];
  protocol::RubyMessagePacket packet;
  if (!packet.ParseFromArray(message->mutable_data(), message->size())) {
    LOG (WARNING) << "The received message is not a valid ruby message packet.";
    return;
  }

  if (!packet.has_message()) {
    LOG(WARNING) << "Received a packet with no message associated.";
    return;
  }

  DispatchMessage(
    socket,
    router_->GetRoutes(message_parts[0]->data(), &packet),
    &packet);
}

void MessageReceiver::DispatchMessage(zmq::Socket* socket,
  const RouteSet& destinations,
  const protocol::RubyMessagePacket* packet) {
  DCHECK(destinations.size());

  int packet_size = packet->ByteSize();
  for (RouteSet::const_iterator destination = destinations.begin();
    destination != destinations.end(); ++destination) {
    // Write the destination address to the router socket as the first
    // message part.
    int destination_address_size = destination->size();
    scoped_refptr<zmq::Message> address(
      new zmq::Message(destination_address_size));
    memcpy(address->mutable_data(), destination->data(),
      destination_address_size);

    // Serialize the message packet into a zmq::Message.
    scoped_refptr<zmq::Message> packet_data(new zmq::Message(packet_size));
    packet->SerializeToArray(packet_data->mutable_data(), packet_size);

    // The message envelope to send a message over a ROUTER->REP/REQ should be.
    //  [DESTINATION ADDRESS]
    //  [EMPTY FRAME]
    //  [DATA]
    socket->Send(address, destination_address_size, zmq::kSendMore);
    socket->Send(scoped_refptr<zmq::Message>(new zmq::Message()), 0,
      zmq::kSendMore);
    socket->Send(packet_data, packet_size, zmq::kNoFlags);
  }
}

}  // namespace node