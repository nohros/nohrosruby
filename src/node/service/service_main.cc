// Copyright (c) 2010 Nohros Systems Inc. All rights reserved
// Use of this source code is governed by BSD-style license that can be found
// in the LICENCE file.
//
// The service can be started as a regular service only, typically at system
// startup. No COM support.
//

#include <base/command_line.h>
#include <base/logging.h>
#include <base/threading/platform_thread.h>
#include <base/basictypes.h>
#include <base/string_number_conversions.h>
#include <base/path_service.h>
#include <base/base_paths.h>
#include <base/file_path.h>
#include <sql/connection.h>

#include "node/service/ruby_service.h"
#include "node/service/ruby_switches.h"
#include "node/service/constants.h"
#include "node/service/services_database.h"
#include "node/service/routing_database.h"
#include "node/service/message_router.h"

int main(int argc, char** argv) {
  CommandLine::Init(argc, argv);
  const CommandLine& switches = *CommandLine::ForCurrentProcess();

  if (switches.HasSwitch(switches::kWaitDebugger)) {
    //base::PlatformThread::Sleep(10000);
  }

  // Set up the services database
  FilePath services_database_path;
  if (!PathService::Get(base::FILE_EXE, &services_database_path)) {
    NOTREACHED();
    return -1;
  }
  services_database_path = services_database_path
    .DirName()
    .Append(node::kServicesDatabaseFilename);
  
  node::ServicesDatabase services_database;
  if (!services_database.Open(services_database_path)) {
    LOG(ERROR) << "Unable to open services database.";
    return -1;
  }

  node::RoutingDatabase routing_database;
  if (!routing_database.Open()) {
    LOG(ERROR) << "Unable to open the routing database";
    return -1;
  }

  node::MessageRouter message_router(&services_database, &routing_database);
  node::RubyService service(&message_router);

  service.Run();
}
