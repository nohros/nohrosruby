// Copyright (c) 2010 Nohros Systems Inc. All rights reserved
// Use of this source code is governed by BSD-style license that can be found
// in the LICENCE file.
//
// The service can be started as a regular service only, typically at system
// startup. No COM support.
//

#include <windows.h>
#include <tchar.h>

#include "third_party/chrome/base/command_line.h"

int main(int argc, char* argv[]) {
  
  // Initialize the commandline singleton from the environment.
  CommandLine::Init(0, NULL);
}
