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
#include <base/threading/platform_thread.h>
#include <base/string_number_conversions.h>
#include <base/memory/scoped_ptr.h>
#include <base/string_util.h>
#include <google/protobuf/repeated_field.h>
#include <sql/connection.h>

#include <control.pb.h>
#include <ruby_protos.pb.h>
#include "node/zeromq/context.h"
#include "node/zeromq/socket.h"
#include "node/zeromq/message.h"
#include "node/service/constants.h"
#include "node/service/service_metadata.h"

namespace node {

typedef google::protobuf::RepeatedPtrField<protocol::KeyValuePair> KeyValuePairSet;

RubyService::RubyService(zmq::Context* context, ServicesDatabase* db)
  : ServiceBase(node::kRubyServiceName),
    context_(context),
    db_(db_),
    router_(NULL),
    service_tracker_address_(""),
    message_channel_port_(node::kMessageChannelPort),
    thread_(NULL),
    is_running_(false) {
}

void RubyService::OnStart(const std::vector<std::wstring>& arguments) {
  VLOG(1) << "Service has been started";

  is_running_ = true ;
  context_->set_error_delegate(this);

  if (!base::PlatformThread::Create(0, this, &thread_)) {
    NOTREACHED() << "Service worker thread creation failed.";
  }
}

// PlatformThread::Delegate() implementation
void RubyService::ThreadMain() {
  // Create our router socket to receive commands from clients and services.
  router_.reset(CreateRouterSocket(message_channel_port_));
  if (router_.get()) {
    while (is_running_) {
      const std::vector<scoped_refptr<zmq::Message>> messages
        = router_.get()->Receive(zmq::kNoFlags);
      if (messages.size() > 0) {
        OnMessage(messages);
      }
    }
  }
}

void RubyService::OnMessage(const MessageParts& message_parts) {
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

  // Route the message to its destination.
  RouteMessage(message_parts[0], &packet);
}

void RubyService::RouteMessage(zmq::Message* sender,
  protocol::RubyMessagePacket* packet) {
  DCHECK(sender);
  DCHECK(packet);

  if (!packet->has_message()) {
    LOG(WARNING) << "Received a packet with no message associated.";
    return;
  }

  // A empty sender means that the message is a request sent to one of
  // the hosted services. In that case we need to set the |sender| value
  // to the address of the message sender and search for the service that
  // should receive the message.
  std::vector<std::string> destinations;
  if (!packet->message().has_sender()) {
    // Set the address of the sender, so message receiver could send a
    // reply back.
    packet->mutable_message()->set_sender(sender->data());

    // Search for the service(s) that should receive the message. If no
    // services are found, reply to the sender with the sent message (modified
    // with the sender address).
    ServiceFacts service_facts = GetServiceFacts(packet->header());
    if (service_facts.size()) {
      ServicesMetadata services;
      if (db_->GetServiceMetadata(service_facts, &services)) {
        DispatchMessage(destinations, packet);
        return;
      }
    }
  }
  destinations.push_back(sender->data());
  DispatchMessage(destinations, packet);
}

void RubyService::DispatchMessage(const std::vector<std::string>& destinations,
  const protocol::RubyMessagePacket* packet) {
  DCHECK(router_.get());
  DCHECK(destinations.size());

  int packet_size = packet->ByteSize();
  for (std::vector<std::string>::iterator destination;
    destination != destinations.begin(); ++destination) {
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
    router_->Send(address, destination_address_size, zmq::kSendMore);
    router_->Send(scoped_refptr<zmq::Message>(new zmq::Message()), 0,
      zmq::kSendMore);
    router_->Send(packet_data, packet_size, zmq::kNoFlags);
  }
}

ServiceFacts RubyService::GetServiceFacts(
  const protocol::RubyMessageHeader& header) {
  ServiceFacts service_facts;
  KeyValuePairSet facts = header.facts();
  for (KeyValuePairSet::iterator fact = facts.begin();
    fact != facts.end(); ++fact) {
    service_facts.push_back(std::make_pair(fact->key(), fact->value()));
  }
  return service_facts;
}

zmq::Socket* RubyService::CreateRouterSocket(int port) {
  std::string endpoint("tcp://*:");
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
  router_->Close();
  context_->Close();

  if (thread_) {
    base::PlatformThread::Join(thread_);
  }
}

int RubyService::OnError(int error, zmq::Context* context, zmq::Socket* socket) {
  LOG(ERROR) << "zmq error " << error
             << ", errno " << context->GetErrorCode()
             << ": " << context->GetErrorMessage();
  return error;
}

RubyService::~RubyService() {
}

}  // namespace node