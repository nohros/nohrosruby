// Copyright (c) 2010 Nohros Systems Inc. All rights reserved
// Use of this source code is governed by BSD-style license that can be found
// in the LICENCE file.

#ifndef NODE_SERVICE_RUBY_SERVICE_H_
#define NODE_SERVICE_RUBY_SERVICE_H_
#pragma once

#include <vector>

#include <base/compiler_specific.h>
#include <base/threading/platform_thread.h>
#include <base/memory/scoped_ptr.h>

#include "node/service/service_base.h"

namespace zmq {
class Context;
class Socket;
}

namespace node {
class MessageRouter;
class MessageReceiver;
class ControlMessageLoop;

class RubyService
    : public ServiceBase {
 public:
  RubyService(MessageRouter* message_router);

  ~RubyService();

 protected:
  // Implementation of the SeviceBase methods.
  void OnStart(const std::vector<std::wstring>& arguments) OVERRIDE;
  void OnStop() OVERRIDE;

 private:
  class ControlMessageThreadDelegate : public base::PlatformThread::Delegate {
   public:
    explicit ControlMessageThreadDelegate(
      ControlMessageLoop* control_message_loop);
    virtual void ThreadMain() OVERRIDE;
   private:
    ControlMessageLoop* control_message_loop_;
  };

  int message_channel_port_;

  base::PlatformThreadHandle control_message_thread_;

  // Store it, because it must outlive the thread.
  scoped_ptr<ControlMessageThreadDelegate> control_message_delegate_;
  scoped_ptr<ControlMessageLoop> control_message_loop_;
  scoped_ptr<MessageReceiver> message_receiver_;
  scoped_ptr<zmq::Context> context_;

  DISALLOW_COPY_AND_ASSIGN(RubyService);
};

}  // namespace node

#endif  // NODE_SERVICE_RUBY_SERVICE_H_