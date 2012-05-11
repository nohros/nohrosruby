// Copyright (c) 2010 Nohros Systems Inc. All rights reserved
// Use of this source code is governed by BSD-style license that can be found
// in the LICENCE file.

#ifndef NODE_SERVICE_RUBY_PATHS_H_
#define NODE_SERVICE_RUBY_PATHS_H_
#pragma once

// This file declares path keys for the ruby module. These can be used with
// the PathService to access various special directories and files.

namespace node {

enum {
  PATH_START = 1000,

  DIR_APP = PATH_START  // Directory where dlls and data reside.
  DIR_LOGS  // Directory where logs should be written.
}

}

#endif  // NODE_SERVICE_RUBY_PATHS_H_