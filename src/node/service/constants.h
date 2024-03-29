// Copyright (c) 2010 Nohros Systems Inc. All rights reserved
// Use of this source code is governed by BSD-style license that can be found
// in the LICENCE file.

// A handful of resource-like constants relaed to the runy node application

#ifndef NODE_SERVICE_CONSTANTS_H_
#define NODE_SERVICE_CONSTANTS_H_
#pragma once

#include <base/file_path.h>

namespace node {

extern const long kMessageChannelPort;
extern const wchar_t kRubyServiceName[];
extern const char kServiceTrackerAddress[];
extern const char kNodeServiceName[];
extern const char kServiceNameFact[];

// filenames
extern const FilePath::CharType kServicesDatabaseFilename[];
extern const FilePath::CharType kServicesDirname[];
extern const FilePath::CharType kServicesHostsDirname[];
extern const FilePath::CharType kNetServiceHostDirname[];
extern const FilePath::CharType kJavaServiceHostDirname[];
extern const FilePath::CharType kPythonServiceHostDirname[];
extern const FilePath::CharType kServiceHostExecutableName[];

}

#endif  // NODE_SERVICE_CONSTANTS_H_
