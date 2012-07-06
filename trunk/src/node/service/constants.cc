// Copyright (c) 2010 Nohros Systems Inc. All rights reserved
// Use of this source code is governed by BSD-style license that can be found
// in the LICENCE file.

#include "node/service/constants.h"

#include "base/file_path.h"

#define FPL FILE_PATH_LITERAL

namespace node {

// The port used to listen for commands.
const long kMessageChannelPort = 8520;

// The address of the service tracker.
const char kServiceTrackerAddress[] = "tcp://127.0.0.1:8520";

const wchar_t kRubyServiceName[] = L"NohrosRuby";

const char kNodeServiceName[] = "ruby";

const FilePath::CharType kServicesDatabaseFilename[] = FPL("services.db");

const FilePath::CharType kServicesDir[] = FPL("services");

}  // namespace node
