// Copyright (c) 2010 Nohros Systems Inc. All rights reserved
// Use of this source code is governed by BSD-style license that can be found
// in the LICENCE file.

#ifndef NODE_SERVICE_RUBY_SERVICE_H_
#define NODE_SERVICE_RUBY_SERVICE_H_

#include <vector>
#include <string>

#include <base/basictypes.h>

#include "node/service/service_base.h"

namespace base {
class FilePath;
}

namespace node {

class RubyService : ServiceBase {
 public:
  RubyService(const char* service_name);
  ~RubyService();

  // Implementation of the SeviceBase methods.
  void OnStart(const std::vector<std::string>& arguments) OVERRIDE;
  void OnStop() OVERRIDE;

  // Creates a new process that runs the application that can communicate with
  // the external word and host service-like applications.
  //void CreateServiceHostProcess(const FilePath& service_host_path);

 private:
};

}  // namespace node

#endif  // NODE_SERVICE_RUBY_SERVICE_H_