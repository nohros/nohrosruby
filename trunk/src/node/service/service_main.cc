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
#include <sql/connection.h>

#include "node/zeromq/context.h"
#include "node/service/ruby_service.h"
#include "node/service/ruby_switches.h"
#include "node/service/constants.h"
#include "node/service/services_database.h"

int main(int argc, char** argv) {
  CommandLine::Init(argc, argv);
  const CommandLine& switches = *CommandLine::ForCurrentProcess();

  if (switches.HasSwitch(switches::kWaitDebugger)) {
    base::PlatformThread::Sleep(10000);
  }

  zmq::Context context;
  if (!context.Open(1)) {
    LOG(ERROR) << "zmq error, " << context.GetErrorMessage();
    return -1;
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

  // Set up the ruby service node.
  node::RubyService service(&context, &services_database);

  int message_channel_port = node::kMessageChannelPort;
  if (switches.HasSwitch(switches::kMessageChannelPort)) {
    std::string value =
      switches.GetSwitchValueASCII(switches::kMessageChannelPort);
    if (!base::StringToInt(value, &message_channel_port)) {
      LOG(WARNING) << "Failed to parse the request reply port: "
                   << value
                   << ". Using the default port: "
                   << node::kMessageChannelPort;
    }
  }
  service.set_message_channel_port(message_channel_port);

  // Set up the service tracker address, if supplied.
  if (switches.HasSwitch(switches::kServiceTrackerAddress)) {
    service.set_service_tracker_address(
      switches.GetSwitchValueASCII(switches::kServiceTrackerAddress));
  }

  service.Run();
}
