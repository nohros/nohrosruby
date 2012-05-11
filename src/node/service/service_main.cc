// Copyright (c) 2010 Nohros Systems Inc. All rights reserved
// Use of this source code is governed by BSD-style license that can be found
// in the LICENCE file.
//
// The service can be started as a regular service only, typically at system
// startup. No COM support.
//

#include "node/service/service_main.h"

#include <string>
#include <windows.h>

#include <base/logging.h>
#include <base/command_line.h>
#include <base/memory/scoped_ptr.h>
#include <base/string_util.h>

#include "node/service/ruby_switches.h"
#include "node/service/service.h"