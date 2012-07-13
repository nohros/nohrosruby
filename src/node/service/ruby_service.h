// Copyright (c) 2010 Nohros Systems Inc. All rights reserved
// Use of this source code is governed by BSD-style license that can be found
// in the LICENCE file.

#ifndef NODE_SERVICE_RUBY_SERVICE_H_
#define NODE_SERVICE_RUBY_SERVICE_H_
#pragma once

#include <vector>

#include <base/compiler_specific.h>
#include <base/threading/platform_thread.h>

#include "node/zeromq/context.h"
#include "node/service/service_base.h"


namespace zmq {
class Socket;
class Message;
}

namespace protocol {
class RubyMessagePacket;
class RubyMessageHeader;

namespace node {
class ServicesDatabase;

typedef std::vector<scoped_refptr<zmq::Message>> MessageParts;

class RubyService
    : public ServiceBase,
      public base::PlatformThread::Delegate,
      public zmq::ErrorDelegate {
 public:
  RubyService(zmq::Context* context, ServicesDatabase* db);

  ~RubyService();

  // Pre-Init configuration ------------------------------------------------

  // Sets the ports numbers that is used by sockets to receives commands
  // from clients and services.
  void set_message_channel_port (int port) { message_channel_port_ = port; }

 protected:
  // Implementation of the SeviceBase methods.
  void OnStart(const std::vector<std::wstring>& arguments) OVERRIDE;
  void OnStop() OVERRIDE;

  // PlatformThread::Delegate implementation.
  virtual void ThreadMain() OVERRIDE;

  // zmq::ErrorDelegate implementation
  int OnError(int error, zmq::Context* context, zmq::Socket* socket);

 private:
  // Process the message parts.
  void OnMessage(const MessageParts& message_parts);
  
  // Dispatches a message packet to its destiantion.
  void DispatchMessage(const std::vector<std::string>& destinations,
    const protocol::RubyMessagePacket* packet);

  // Creates a socket object and bind it to the given |port|.Returns true
  // if the socket was created succesfully; otherwise, false.
  zmq::Socket* CreateRouterSocket(int port);
  zmq::Context* context_;

  bool is_running_;
  int message_channel_port_;

  // The zeromq socket that is used as a message router.
  scoped_ptr<zmq::Socket> router_;

  // Worker thread handle.
  base::PlatformThreadHandle thread_;

  DISALLOW_COPY_AND_ASSIGN(RubyService);
};

}  // namespace node

#endif  // NODE_SERVICE_RUBY_SERVICE_H_