// Copyright (c) 2010 Nohros Systems Inc. All rights reserved
// Use of this source code is governed by BSD-style license that can be found
// in the LICENCE file.

#ifndef NODE_SERVICE_SERVICE_METADATA_H_
#define NODE_SERVICE_SERVICE_METADATA_H_
#pragma once

#include <string>

#include <base/basictypes.h>
#include <base/memory/ref_counted.h>

namespace node {

enum LanguageRuntimeType {
  kNet = 1,
  kJava = 2,
  kMachineCode = 3,
  kPython = 4
};

// Holds all informations associated with a specific service. Refcounting
// allow us to give these metadata out to ServicesDatabase object.
class ServiceMetadata : public base::RefCountedThreadSafe<ServiceMetadata> {
 public:
  ServiceMetadata();
  ServiceMetadata(int service_id,
                  std::string service_name,
                  LanguageRuntimeType language_runtime);
  ServiceMetadata(int service_id,
                  std::string service_name,
                  LanguageRuntimeType language_runtime,
                  std::string arguments);

  // A string that uniquely identifies a service within a node for a specific
  // language runtime.
  const std::string& service_name() const { return service_name_; }
  void set_service_name(const std::string& service_name) {
    service_name_ = service_name;
  }
  void set_service_name(const char* service_name) {
    service_name_ = std::string(service_name);
  }

  // Any command line arguments or properties required to run the service.
  const std::string& arguments() const { return arguments_; }
  void set_arguments(const std::string arguments) { arguments_ = arguments; }
  void set_arguments(const char* arguments) {
    arguments_ = std::string(arguments);
  }

  // The language runtime associated with the service.
  int language_runtime_type() const { return language_runtime_type_; }
  void set_language_runtime_type(int language_runtime_type) {
    language_runtime_type_ = language_runtime_type;
  }

  // The internal service ID.
  int service_id() const { return service_id_; }
  void set_service_id(int service_id) {
    service_id_ = service_id;
  }

 private:
  friend class base::RefCountedThreadSafe<ServiceMetadata>;
  ~ServiceMetadata();

  std::string service_name_;
  std::string arguments_;
  int language_runtime_type_;
  int service_id_;

  DISALLOW_COPY_AND_ASSIGN(ServiceMetadata);
};

}  // namespace node

#endif  // NODE_SERVICE_SERVICE_METADATA_H_