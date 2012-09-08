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

// Overrides the default port used for commands delivery.
const char kMessageChannelPort[] = "message-channel-port";

// Specifies the aaddress of the service tracker.
const char kServiceTrackerAddress[] = "service-tracker-address";

const char kWaitDebugger[] = "wait-debugger";

const char kLaunchDebug[] = "debug";

}  // namespace swicthes