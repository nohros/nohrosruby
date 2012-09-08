// Copyright (c) 2010 Nohros Systems Inc. All rights reserved
// Use of this source code is governed by BSD-style license that can be found
// in the LICENCE file.

#ifndef NODE_SERVICE_NODE_MESSAGE_LOOP_H_
#define NODE_SERVICE_NODE_MESSAGE_LOOP_H_
#pragma once

#include <base/basictypes.h>
#include <base/compiler_specific.h>
#include <base/threading/platform_thread.h>
#include <base/file_path.h>
#include <base/memory/scoped_ptr.h>
#include <base/memory/ref_counted.h>

namespace zmq {
class Context;
class Socket;
class Message;
}

class FilePath;

namespace node {
class MessageRouter;

// A NodeMessageLoop is used to process messages sent to the service node.
class NodeMessageLoop {
 public:
  typedef std::vector<scoped_refptr<zmq::Message>> MessageParts;

  NodeMessageLoop(zmq::Context* context, MessageRouter* message_router);
  ~NodeMessageLoop();

  // Run the NodeMessageLoop. This blocks until Quit is called.
  void Run();

  bool running() const { return running_; }
  
  // Quit an earlier call to Run(). Quit can be called before, during or after
  // Run. If called before Run, Run will return imediately when called. Calling
  // Quit after the NodeMessageLoop has already finished running has no
  // effect.
  void Quit();

  // Sets the ports numbers that is used by sockets to receives commands
  // from clients and services. If not called the default port will be used.
  void set_message_channel_port (int port) { message_channel_port_ = port; }

 private:
  // Register out self into the routing database. We need to be registered
  // into the routing database in order to start receiving messages.
  bool NodeMessageLoop::RegisterRoute();

  // Method that is called when a message is received.
  void OnMessageReceived(const MessageParts& message_parts);

  zmq::Context* context_;
  MessageRouter* message_router_;

  // The zeromq socket that is used as a message router.
  scoped_ptr<zmq::Socket> dealer_;

  // The directory where the services are stored.
  const FilePath services_base_dir_;
  int message_channel_port_;

  bool running_;
  bool run_called_;
  bool quit_called_;
};

}  // namespace node

#endif  // NODE_SERVICE_NODE_MESSAGE_LOOP_