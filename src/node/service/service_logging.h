// Copyright (c) 2010 Nohros Systems Inc. All rights reserved
// Use of this source code is governed by BSD-style license that can be found
// in the LICENCE file.
//

#include <base/logging.h>

#define LOG_SERVICE_START(severity, service_name)  \
  LOG(severity) \
    << "The Service " << #service_name \
    << "fails to start with error code:" \
    << logging::GetLastSystemErrorCode();
