// Copyright (c) 2010 Nohros Systems Inc. All rights reserved
// Use of this source code is governed by BSD-style license that can be found
// in the LICENCE file.

#include "node/service/constants.h"

namespace node {

// The port used to listen for commands from clients.
const long kFrontendPort = 8520;

// The port used to listen for commands from services.
const long kBackendPort = 8521;

const char kRubyServiceName[] = "NohrosRuby";

}  // namespace node
