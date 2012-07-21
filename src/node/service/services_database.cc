// Copyright (c) 2010 Nohros Systems Inc. All rights reserved
// Use of this source code is governed by BSD-style license that can be found
// in the LICENCE file.

#include "node/service/services_database.h"

#include <base/logging.h>
#include <base/file_util.h>
#include <base/stringprintf.h>
#include <base/string_number_conversions.h>
#include <sql/connection.h>
#include <sql/transaction.h>
#include <sql/statement.h>

#include "node/service/hash.h"
#include "node/service/service_metadata.h"

namespace node {

static const int kVersionNumber = 1;

ServicesDatabase::ServicesDatabase() {
}

ServicesDatabase::~ServicesDatabase() {
}

bool ServicesDatabase::Open(const FilePath& db_name) {
  bool file_existed = file_util::PathExists(db_name);

  db_.reset(CreateDB(db_name));
  if (!db_.get()) {
    return false;
  }

  bool does_meta_exist = sql::MetaTable::DoesTableExist(db_.get());
  if (!does_meta_exist && file_existed) {
    // If the metadata does not exist, this version is old or invalid. We
    // could remove all the entries as they are no longer applicable, but
    // it's safest to just remove the file and start over.
    db_.reset(NULL);
    if (!file_util::Delete(db_name, false) &&
        !file_util::Delete(db_name, false)) {
      // Try to delete twice. If we can't, fail.
      LOG(ERROR) << "Unable to delete old Services database file.";
      return false;
    }
    db_.reset(CreateDB(db_name));
    if (!db_.get()) {
      return false;
    }
  }

  // Scope initialization in a transaction so we can't be partially
  // initialized.
  sql::Transaction transaction(db_.get());
  transaction.Begin();

  if (!meta_table_.Init(db_.get(), kVersionNumber, kVersionNumber)) {
    return false;
  }

  if (!InitServicesTable()) {
    return false;
  }

  // Initialization is complete.
  if (!transaction.Commit()) {
    return false;
  }

  return true;
}

bool ServicesDatabase::InitServicesTable() {
  if (!db_->DoesTableExist("services")) {
    if (!db_->Execute("CREATE TABLE services ("
                      "id INTEGER PRIMARY KEY,"
                      "name VARCHAR NOT NULL,"
                      "working_dir VARCHAR NOT NULL,"
                      "language_runtime_type INTEGER DEFAULT 1,"
                      "arguments VARCHAR NOT NULL)")) {
      LOG(WARNING) << db_->GetErrorMessage();
      return false;
    }
  }
  return true;
}

bool ServicesDatabase::InitServicesFactsTable() {
  return db_->DoesTableExist("facts") ||
    (db_->Execute("CREATE TABLE facts ("
                  "id INTEGER PRIMARY,"
                  "hash_code INTEGER NOT NULL,"
                  "service_id INTEGER NOT NULL)") &&
    db_->Execute("CREATE INDEX IF NOT EXISTS facts_hash_code"
                 "ON facts(hash_code)"));
}

bool ServicesDatabase::GetServicesMetadata(const ServiceFactSet& facts,
  ServicesMetadataSet* services) {
  DCHECK(db_.get());
  DCHECK(services);
  DCHECK(facts.size());

  int facts_size = facts.size();

  uint32 first_service_fact_hash = GetServiceFactHash(facts[0]);

  // First get all the services that has the first fact.
  sql::Statement s(db_->GetCachedStatement(SQL_FROM_HERE,
    "SELECT service_id FROM facts WHERE hash_code = ?"));
  s.BindInt(0, first_service_fact_hash);

  std::set<int> services_found;
  while (s.Step()) {
    services_found.insert(s.ColumnInt(0));
  }

  std::string cmd("SELECT id, name, language_runtime_type, working_dir, arguments "
                  "FROM services s "
                  "INNER JOIN facts f on f.service_id = s.id "
                  "WHERE service_id = ? and hash_code in (");

  cmd += base::IntToString(first_service_fact_hash);
  for (int i = 1; i< facts_size; ++i) {
    cmd += "," +  base::IntToString(GetServiceFactHash(facts[i]));
  }
  cmd += ")";

  sql::Statement statement(db_->GetCachedStatement(SQL_FROM_HERE, cmd.c_str()));
  for (std::set<int>::iterator k = services_found.begin();
        k != services_found.end(); ++k) {
    statement.BindInt(0, *k);
    if (!statement.Step()) {
      return false;
    }

    scoped_refptr<ServiceMetadata> service(new ServiceMetadata());
    service->set_service_id(statement.ColumnInt(0));
    service->set_service_name(statement.ColumnString(1));
    service->set_language_runtime_type(
      static_cast<LanguageRuntimeType>(statement.ColumnInt(2)));
    service->set_service_working_dir(statement.ColumnString(3));
    service->set_arguments(statement.ColumnString(4));

    services->push_back(service);
    statement.Reset();
  }
  return services->size() > 0;
}

uint32 ServicesDatabase::GetServiceFactHash(const std::pair<std::string,
  std::string> fact) {
  return node::Hash(base::StringPrintf("%s=%s", fact.first.data(),
    fact.second.data()));
}

sql::Connection* ServicesDatabase::CreateDB(const FilePath& db_name) {
  scoped_ptr<sql::Connection> db(new sql::Connection);

  // Services database only stores metadata information about installed
  // services, so we don't need that big a page size or cache.
  db->set_page_size(4096);
  db->set_cache_size(32);

  // Run the database in exclusive mode. Nobody else should be accessing
  // the database while we're running, and this will give somewhat improved
  // perf.
  //db->set_exclusive_locking();

  if (!db->Open(db_name)) {
    LOG(ERROR) << db->GetErrorMessage();
    return NULL;
  }
  return db.release();
}

}  // namespace node