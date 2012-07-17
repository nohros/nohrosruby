// Copyright (c) 2010 Nohros Systems Inc. All rights reserved
// Use of this source code is governed by BSD-style license that can be found
// in the LICENCE file.

#include "node/service/message_router.h"

#include <base/logging.h>
#include <sql/connection.h>
#include <google/protobuf/repeated_field.h>
#include <ruby_protos.pb.h>

#include "node/service/routing_database.h"

namespace node {

typedef 
  google::protobuf::RepeatedPtrField<ruby::KeyValuePair> KeyValuePairSet;

MessageRouter::MessageRouter(ServicesDatabase* services_database,
  RoutingDatabase* routing_database)
  : services_database_(services_database),
    routing_database_(routing_database) {
  DCHECK(services_database);
  DCHECK(routing_database);
}

MessageRouter::~MessageRouter() {
}

RouteSet MessageRouter::GetRoutes(const std::string& sender,
  protocol::RubyMessagePacket* packet) {
  DCHECK(packet);

  RouteSet routes;

  // A empty sender means that the message is a request sent to one of
  // the hosted services. In that case we need to set the |sender| value
  // to the address of the message sender and search for the service that
  // should receive the message.
  if (!packet->message().has_sender()) {
    // Set the address of the sender, so message receiver could send a
    // reply back.
    packet->mutable_message()->set_sender(sender);

    // Search for the service(s) that should receive the message. If no
    // services are found, reply to the sender with the sent message (modified
    // with the sender address).
    ServiceFactSet service_facts = GetServiceFacts(packet->header());
    if (service_facts.size() > 0) {
      ServicesMetadataSet services;
      if (services_database_->GetServicesMetadata(service_facts, &services)) {
        // We found services that matches the given facts in our database,
        // now we need to check if the found services are running and get its
        // addresses.
        for (ServicesMetadataSet::iterator service = services.begin();
          service != services.end(); ++service) { 
        }
      }
    }
  }
  routes.push_back(sender);
  return routes;
}

bool MessageRouter::AddRoute(const std::string& address,
  const ServiceFactSet& facts) {
  DCHECK(facts.size());
  ServicesMetadataSet services;
  if (!services_database_->GetServicesMetadata(facts, &services)) {
    LOG(WARNING) << "Attempt to add a route to an unregistered service";
    return false;
  }

  for (ServicesMetadataSet::iterator service = services.begin();
    service != services.end(); ++service) {
    if (!routing_database_->AddRoute(service->get()->service_id(), address)) {
      return false;
    }
  }
  return true;
}

ServiceFactSet MessageRouter::GetServiceFacts(
  const protocol::RubyMessageHeader& header) {
  ServiceFactSet service_facts;
  KeyValuePairSet facts = header.facts();
  for (KeyValuePairSet::iterator fact = facts.begin();
    fact != facts.end(); ++fact) {
    service_facts.push_back(std::make_pair(fact->key(), fact->value()));
  }
  return service_facts;
}

}  // namesapce node
