// Copyright (c) 2010 Nohros Systems Inc. All rights reserved
// Use of this source code is governed by BSD-style license that can be found
// in the LICENCE file.

#ifndef NODE_SERVICE_MESSAGE_RECEIVER_H_
#define NODE_SERVICE_MESSAGE_RECEIVER_H_
#pragma once

#include <vector>
#include <string>

#include <base/memory/ref_counted.h>

namespace protocol {
class RubyMessagePacket;
class RubyMessageHeader;
}

namespace zmq {
class Socket;
class Message;
class Context;
}

namespace node {
class MessageRouter;

class MessageReceiver {
 public:
  MessageReceiver(zmq::Context* context, MessageRouter* message_router);
  ~MessageReceiver();

  // Start the MessageReceiver. This blocks until Stop is called.
  void Start();

  // Stop an earlier call to Start(), causing the receiver to stop receiving
  // messages.
  void Stop();

  // Sets the ports numbers that is used by sockets to receives commands
  // from clients and services. If not called the default port will be used.
  void set_message_channel_port (int port) { message_channel_port_ = port; }

 private:
  typedef std::vector<scoped_refptr<zmq::Message>> MessageParts;

  // Process the message parts.
  void OnMessageReceived(zmq::Socket* socket,
    const MessageParts& message_parts);
  
  // Dispatches a message packet to its destiantion.
  void DispatchMessage(zmq::Socket* socket,
    const std::vector<std::string>& destinations,
    const protocol::RubyMessagePacket* packet);

  zmq::Context* context_;
  MessageRouter* router_;

  bool running_;
  int message_channel_port_;
};

}  // namespace node

#endif  // NODE_SERVICE_MESSAGE_RECEIVER_H_