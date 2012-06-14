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

#include "node/zeromq/context.h"
#include "node/service/ruby_service.h"
#include "node/service/ruby_switches.h"
#include "node/service/constants.h"

int main(int argc, char** argv) {
  CommandLine::Init(argc, argv);
  const CommandLine& switches = *CommandLine::ForCurrentProcess();

  if (switches.HasSwitch(switches::kWaitDebugger)) {
    base::PlatformThread::Sleep(10000);
  }

  zmq::Context context;
  context.Open(1);

  int request_reply_port = node::kRequestReplyPort;
  if (switches.HasSwitch(switches::kRequestReplyPort)) {
    std::string value =
      switches.GetSwitchValueASCII(switches::kRequestReplyPort);
    if (!base::StringToInt(value, &request_reply_port)) {
      LOG(ERROR) << "Failed to parse the request reply port" << value;
    }
  }

  node::RubyService service(&context);
  service.set_request_reply_port(request_reply_port);
  service.Run();
}