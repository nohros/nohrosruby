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
#include "node/service/services_database.h"


namespace zmq {
class Socket;
class Message;
}

namespace sql {
class Connection;
}

namespace protocol {
class RubyMessagePacket;
class RubyMessageHeader;

namespace control {
class AnnounceMessage;
}
}

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

  // Sets the address of the service tracker.
  void set_service_tracker_address (const std::string& service_tracker_address) {
    service_tracker_address_ = service_tracker_address;
  }

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
  void OnNodeMessage(const protocol::RubyMessagePacket& packet);
  
  // Routes a message to its final destination. |sender| is the socket address
  // of the message sender and |packet| is the message packet to be routed.
  void RouteMessage(zmq::Message* sender, protocol::RubyMessagePacket* packet);

  // Dispatches a message packet to its destiantion.
  void DispatchMessage(const std::vector<std::string>& destinations,
    const protocol::RubyMessagePacket* packet);

  ServiceFacts GetServiceFacts(const protocol::RubyMessageHeader& header);

  // Creates a socket object and bind it to the given |port|.Returns true
  // if the socket was created succesfully; otherwise, false.
  zmq::Socket* CreateRouterSocket(int port);
  zmq::Context* context_;

  // The database that is used to store the services metadata.
  ServicesDatabase* db_;

  bool is_running_;
  int message_channel_port_;
  std::string service_tracker_address_;

  // The zeromq socket that is used as a message router.
  scoped_ptr<zmq::Socket> router_;

  // Worker thread handle.
  base::PlatformThreadHandle thread_;

  DISALLOW_COPY_AND_ASSIGN(RubyService);
};

}  // namespace node

#endif  // NODE_SERVICE_RUBY_SERVICE_H_