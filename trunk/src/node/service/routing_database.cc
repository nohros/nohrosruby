// Copyright (c) 2010 Nohros Systems Inc. All rights reserved
// Use of this source code is governed by BSD-style license that can be found
// in the LICENCE file.

#include "node/service/routing_database.h"

#include <base/logging.h>
#include <sql/connection.h>
#include <sql/statement.h>
#include <sql/transaction.h>

namespace node {

RoutingDatabase::RoutingDatabase()
  : db_(new sql::Connection()) {
}

RoutingDatabase::~RoutingDatabase() {
}

bool RoutingDatabase::Open() {
  if (!db_->OpenInMemory()) {
    return false;
  }

  // Scope initialization in a transaction so we can't be partially initialized
  sql::Transaction transaction(db_.get());
  transaction.Begin();

  if (!InitRoutesTable()) {
    return false;
  }

  // Initialization is complete.
  if (!transaction.Commit()) {
    return false;
  }

  return true;
}

bool RoutingDatabase::AddRoute(int service_id, const std::string& address) {
  sql::Statement s(db_->GetUniqueStatement(
    "INSERT INTO routes(service_id, address) VALUES(?, ?)"));
  s.BindInt(0, service_id);
  s.BindBlob(1, address.data(), address.size());
  return s.Run();
}

bool RoutingDatabase::RemoveRoute(int service_id) {
  sql::Statement s(db_->GetUniqueStatement(
    "DELETE FROM routes WHERE service_id = ?"));
  s.BindInt(0, service_id);
  return s.Run();
}

bool RoutingDatabase::GetRoute(int service_id, std::string* address) {
  sql::Statement s(db_->GetCachedStatement(SQL_FROM_HERE,
    "SELECT address FROM routes WHERE service_id = ?"));
  if (s.Step()) {
    address->swap(s.ColumnString(0));
    return true;
  }
  return false;
}

bool RoutingDatabase::InitRoutesTable() {
  if (!db_->DoesTableExist("routes")) {
    if (!db_->Execute("CREATE TABLE routes ("
                      "service_id INTEGER PRIMARY KEY,"
                      "address BLOB NOT NULL)")) {
      LOG(WARNING) << db_->GetErrorMessage();
      return false;
    }
  }
  return true;
}

}  // namespace node