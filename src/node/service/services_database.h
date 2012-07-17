// Copyright (c) 2010 Nohros Systems Inc. All rights reserved
// Use of this source code is governed by BSD-style license that can be found
// in the LICENCE file.

#ifndef NODE_SERVICE_SERVICES_DATABASE_H_
#define NODE_SERVICE_SERVICES_DATABASE_H_
#pragma once

#include <vector>
#include <string>

#include <sql/meta_table.h>
#include <base/file_path.h>
#include <base/basictypes.h>
#include <base/memory/scoped_ptr.h>
#include <base/memory/ref_counted.h>

#include "node/service/service_metadata.h"

namespace sql {
class Connection;
}

namespace protocol {
class KeyValuePair;
}

namespace node {
class ServiceMetadata;

typedef std::pair<std::string, std::string> ServiceFact;
typedef std::vector<ServiceFact> ServiceFactSet;
typedef std::vector<scoped_refptr<ServiceMetadata> > ServicesMetadataSet;

class ServicesDatabase {
 public:
  ServicesDatabase();
  ~ServicesDatabase();

  // Must be called after creation but before any other methods are called.
  // Returns true on success. If false, no other functions should be called.
  bool Open(const FilePath& db_name);

  // Services metadata ------------------------------------------------------

  // Gets the metadata for the services that has the given facts. Returns true
  // if we have metadata for at least one service associated with the given
  // facts.
  bool GetServicesMetadata(const ServiceFactSet& facts,
    ServicesMetadataSet* medatada);

 private:
  // Creates the services table, returning true if the table already exists
  // or was successfully created.
  bool InitServicesTable();

  // Creates the facts table, returning true if the table already exists
  // or was successfully created.
  bool InitServicesFactsTable();

  uint32 GetServiceFactHash(const std::pair<std::string, std::string> fact);

  sql::Connection* CreateDB(const FilePath& db_name);

  scoped_ptr<sql::Connection> db_;
  sql::MetaTable meta_table_;

  DISALLOW_COPY_AND_ASSIGN(ServicesDatabase);
};

}  // namesapce node

#endif  // NODE_SERVICE_SERVICES_DATABASE_H_