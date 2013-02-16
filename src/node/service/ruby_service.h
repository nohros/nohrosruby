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
class MessageLoop;
class ServicesDatabase;
class RoutingDatabase;

class RubyService : public ServiceBase {
 public:
  RubyService();
  ~RubyService();

 protected:
  // Implementation of the SeviceBase methods.
  bool OnStart(const std::vector<std::wstring>& arguments) OVERRIDE;
  void OnStop() OVERRIDE;

 private:
  class MessageLoopThreadDelegate : public base::PlatformThread::Delegate {
   public:
    explicit MessageLoopThreadDelegate(MessageLoop* control_message_loop);
    virtual void ThreadMain() OVERRIDE;
   private:
    MessageLoop* message_loop_;
  };

  class ServiceThreadDelegate : public base::PlatformThread::Delegate {
   public:
    explicit ServiceThreadDelegate(RubyService* service);
    virtual void ThreadMain() OVERRIDE;
   private:
    RubyService* service_;
  };

  // Member initialization functions.
  bool InitializeContext();
  bool InitializeMessageLoop();
  bool InitializeMessageReceiver();
  void Run();

  int message_channel_port_;

  base::PlatformThreadHandle service_thread_;

  // Store it, because it must outlive the thread.
  scoped_ptr<ServiceThreadDelegate> service_thread_delegate_;
  scoped_ptr<MessageLoop> message_loop_;
  scoped_ptr<MessageReceiver> message_receiver_;
  scoped_ptr<zmq::Context> context_;
  scoped_ptr<MessageRouter> message_router_;

  // Databases
  scoped_ptr<ServicesDatabase> services_db_;
  scoped_ptr<RoutingDatabase> routing_db_;

  DISALLOW_COPY_AND_ASSIGN(RubyService);
};

}  // namespace node

#endif  // NODE_SERVICE_RUBY_SERVICE_H_