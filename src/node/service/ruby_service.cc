// Copyright (c) 2010 Nohros Systems Inc. All rights reserved
// Use of this source code is governed by BSD-style license that can be found
// in the LICENCE file.

#include "node/service/ruby_service.h"

#include <windows.h>

#include <base/file_path.h>
#include <base/command_line.h>
#include <base/logging.h>

#include "node/service/ruby_switches.h"

namespace node {

RubyService::RubyService(const char* service_name)
  : ServiceBase(service_name) {
}

void RubyService::OnStart(const std::vector<std::string>& arguments) {
  CommandLine* command_line = CommandLine::ForCurrentProcess();
  if (!command_line->HasSwitch(switches::kServiceHost)) {
    LOG(ERROR) << "The path for the service host was not specified.";
    return;
  }

  FilePath service_host_path = command_line->GetSwitchValuePath(
    switches::kServiceHost);
  std::string service_host_cmd_suffix = command_line->GetSwitchValueASCII(
    switches::kServiceHostCmdSuffix);
  
  CreateProcess(NULL,
    const_cast<wchat_t*>service_host_path.value().c_str());
}

}  // namespace node