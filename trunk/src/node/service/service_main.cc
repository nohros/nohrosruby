// Copyright (c) 2010 Nohros Systems Inc. All rights reserved
// Use of this source code is governed by BSD-style license that can be found
// in the LICENCE file.
//
// The service can be started as a regular service only, typically at system
// startup. No COM support.
//

#include <base/command_line.h>

#include "node/service/ruby_service.h"
#include "node/service/ruby_switches.h"
#include "node/service/ruby_service.h"

int main(int argc, char** argv) {
  CommandLine::Init(argc, argv);
  const CommandLine& switches = *CommandLine::ForCurrentProcess();

  if (switches.HasSwitch(switches::kLaunchDebug)) {
    DebugBreak();
  }

  node::RubyService service;
  service.Start();
}
