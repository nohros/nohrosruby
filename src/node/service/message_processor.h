// Copyright (c) 2010 Nohros Systems Inc. All rights reserved
// Use of this source code is governed by BSD-style license that can be found
// in the LICENCE file.

#ifndef NODE_SERVICE_MESSAGE_PROCESSOR_H_
#define NODE_SERVICE_MESSAGE_PROCESSOR_H_
#pragma once

#include <base/basictypes.h>
#include <base/compiler_specific.h>
#include <base/threading/platform_thread.h>

#include "node/zeromq/context.h"

namespace zmq {
class Socket;
}

namespace node {

class MessageProcessor
  : public base::PlatformThread::Delegate,
    public zmq::ErrorDelegate {
 public:
  explicit MessageProcessor(zmq::Context* context);
  ~MessageProcessor();

  // Runs the message processor. This method returns the control to the caller
  // as soon as the thread that performs the message processing in background
  // is created.
  void Run();

  // Sets the ports numbers that is used by sockets to receives commands
  // from clients and services.
  void set_message_channel_port (int port) { message_channel_port_ = port; }

 protected:
  // PlatformThread::Delegate implementation.
  virtual void ThreadMain() OVERRIDE;

  // zmq::ErrorDelegate implementation
  int OnError(int error, zmq::Context* context, zmq::Socket* socket);

 private:
  zmq::Context* context_;

  // The zeromq socket that is used as a message router.
  scoped_ptr<zmq::Socket> dealer_;

  int message_channel_port_;
};

}  // namespace node

#endif  // NODE_SERVICE_MESSAGE_PROCESSOR_H_