// Copyright (c) 2010 Nohros Systems Inc. All rights reserved
// Use of this source code is governed by BSD-style license that can be found
// in the LICENCE file.

#ifndef NODE_SERVICE_MESSAGE_ROUTER_H_
#define NODE_SERVICE_MESSAGE_ROUTER_H_
#pragma once

#include <string>
#include <vector>

#include "node/service/services_database.h"

namespace protocol {
class RubyMessagePacket;
}

namespace sql {
class Connection;
}

namespace node {
class ServicesDatabase;

typedef std::vector<std::string> RouteSet;

// The message router handles all incoming messages sent to the node service
// by routing them to the correct service. Routing is based on service facts.
//
// When a message arrives, the service facts is used to find the IDs of the
// services that should receive the message. These IDs is used to index the
// set of routes to find the services address. If a route is not found the
// message is sent back to the sender.
//
class MessageRouter {
 public:
  explicit MessageRouter(ServicesDatabase* service_database);
  ~MessageRouter();

  // Gets the routes for a message. |sender| is the the sender ID and |packet|
  // is the message packet that need to be routed.
  RouteSet GetRoutes(const std::string& sender,
    protocol::RubyMessagePacket* packet);

 private:
  ServiceFactSet GetServiceFacts(const protocol::RubyMessageHeader& header);

  // The database used to store information about the installed services.
  ServicesDatabase* services_database_;
};

}  // namesapce node

#endif  // NODE_SERVICE_MESSAGE_ROUTER_H_