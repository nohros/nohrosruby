// Copyright (c) 2010 Nohros Systems Inc. All rights reserved
// Use of this source code is governed by BSD-style license that can be found
// in the LICENCE file.

#include "node/service/service_metadata.h"

namespace node {

ServiceMetadata::ServiceMetadata() {
}

ServiceMetadata::ServiceMetadata(
  int service_id,
  const std::string& service_name,
  LanguageRuntimeType language_runtime_type,
  const std::string& service_working_dir)
  : service_id_(service_id),
    service_name_(service_name),
    language_runtime_type_(language_runtime_type),
    service_working_dir_(service_working_dir) {
}

ServiceMetadata::~ServiceMetadata() {
}

}  // namespace node
