// Copyright (c) 2010 Nohros Systems Inc. All rights reserved
// Use of this source code is governed by BSD-style license that can be found
// in the LICENCE file.

#include "node/service/ruby_switches.h"

namespace switches {

// ---------------------------------------------------------------------------
// When commenting your switch, please use the same voice as surrounding
// comments. Imagine "This switch..." at the beginning of the phrase, and it'll
// all work out.
// ---------------------------------------------------------------------------

// Specifies the path to the service host executable.
const char kServiceHost[] = "service-host";

// The contents of this flag are appended to the service host command line.
const char kServiceHostCmdSuffix[] = "service-host-cmd-suffix";

}  // namespace swicthes