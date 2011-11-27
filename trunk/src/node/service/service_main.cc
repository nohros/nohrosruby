// Copyright (c) 2010 Nohros Systems Inc. All rights reserved
// Use of this source code is governed by BSD-style license that can be found
// in the LICENCE file.
//
// The service can be started as a regular service only, typically at system
// startup. No COM support.
//

#include <windows.h>
#include <tchar.h>

#include "service.h"
#include "common/ruby_switches.h"
#include "third_party/chrome/base/logging.h"
#include "third_party/chrome/base/command_line.h"

int main(int argc, char* argv[]) {
  
  // Initialize the commandline singleton.
  CommandLine::Init(argc, argv);

  const CommandLine& command_line = *CommandLine::ForCurrentProcess();

  Service service;
  if (service.Initialize(command_line)) {
    service.Run();
  } else {
    LOG(ERROR) << "The service failed to initialize.";
  }

  if(command_line.HasSwitch(switches::kShutdownService)) {
    service.Shutdown();
  }

  service.Teardown();
}
