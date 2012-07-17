// Copyright (c) 2010 Nohros Systems Inc. All rights reserved
// Use of this source code is governed by BSD-style license that can be found
// in the LICENCE file.

#ifndef NODE_SERVICE_ROUTING_DATABASE_H_
#define NODE_SERVICE_ROUTING_DATABASE_H_
#pragma once

#include <string>

#include <base/memory/scoped_ptr.h>

namespace sql {
class Connection;
}

namespace node {

// A in-memory sql database used to store the routes to the running services.
class RoutingDatabase {
 public:
  RoutingDatabase();
  ~RoutingDatabase();

  // Must be called after creation but before any other methods are called.
  // Returns true on success. If false, no other functions should be called.
  bool Open();

  // Associates the |address| with the service which ID is |service_id|.
  // Returns true when the association is successfully performed; otherwise,
  // false.
  bool AddRoute(int service_id, const std::string& address);

  // Removes the service associated with the ID |service_id| from the routing
  // table. Returns true when the entry is successfully removed, false
  // otherwise.
  bool RemoveRoute(int service_id);

  // Gets the route address for the service associated with |service_id|.
  // Returns true when a route exists; otherwise, false.
  bool GetRoute(int service_id, std::string* address);

 private:
  // Creates the routes table, returning true if the table already exists or
  // was successfully created.
  bool InitRoutesTable();

  scoped_ptr<sql::Connection> db_;
};

}  // namespace node

#endif  // NODE_SERVICE_ROUTING_DATABASE_H_