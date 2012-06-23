// Copyright (c) 2010 Nohros Systems Inc. All rights reserved
// Use of this source code is governed by BSD-style license that can be found
// in the LICENCE file.

#ifndef NODE_SERVICE_RUBY_SERVICE_H_
#define NODE_SERVICE_RUBY_SERVICE_H_
#pragma once

#include <vector>

#include <base/compiler_specific.h>

#include "node/service/service_base.h"
#include <base/threading/platform_thread.h>

namespace zmq {
class Context;
class Socket;
}

namespace node {

class RubyService
    : public ServiceBase,
      public base::PlatformThread::Delegate {
 public:
  RubyService(zmq::Context* context);

  ~RubyService();

  // Pre-Init configuration ------------------------------------------------

  // Sets the ports numbers that is used by sockets to receives commands
  // from clients and services.
  void set_message_channel_port (int port) { message_channel_port_ = port; }

 protected:
  // Implementation of the SeviceBase methods.
  void OnStart(const std::vector<std::string>& arguments) OVERRIDE;
  void OnStop() OVERRIDE;

  // PlatformThread::Delegate implementation.
  virtual void ThreadMain() OVERRIDE;

 private:
  // Creates a socket object and bind it to the given |port|.Returns true
  // if the socket was created succesfully; otherwise, false.
  zmq::Socket* CreateSocket(int port);
  zmq::Context* context_;
  int message_channel_port_;

  bool is_running_;

  // Worker thread handle.
  base::PlatformThreadHandle thread_;

  DISALLOW_COPY_AND_ASSIGN(RubyService);
};

}  // namespace node

#endif  // NODE_SERVICE_RUBY_SERVICE_H_