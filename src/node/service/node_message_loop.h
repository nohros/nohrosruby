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

namespace ruby {
namespace protocol {
namespace control {
class AnnounceMessage;
class QueryMessage;
}
class RubyMessage;
}
}

class FilePath;

namespace node {
class MessageRouter;
class ServicesDatabase;

// A NodeMessageLoop is used to process messages sent to the service node. It
// waits a message to be sent over the message channel, process it and delivers
// it to the appropriate service if needed.
class MessageLoop {
 public:
  typedef std::vector<scoped_refptr<zmq::Message>> MessageParts;

  // Error codes during message processing.
  enum ProcessingError {
    RUBY_CONTROL_NO_ERROR = 0,
    RUBY_CONTROL_INVALID_MESSAGE = 1
  };

  // String version of message processing error codes.
  static const char* kInvalidMessage;
  static const char* kInvalidErrorCode;

  MessageLoop(zmq::Context* context, MessageRouter* message_router,
    ServicesDatabase* services_db);
  ~MessageLoop();

  // Run the NodeMessageLoop. This blocks until Quit is called.
  void Run();

  bool running() const { return running_; }
  
  // Quit an earlier call to Run(). Quit can be called before, during or after
  // Run. If called before Run, Run will return imediately when called. Calling
  // Quit after the NodeMessageLoop has already finished running has no
  // effect.
  void Quit();

  // Set the port number that is used to receives commands from clients and
  // service. If this method not called the default port will be used.
  void set_message_channel_port (int port) { message_channel_port_ = port; }

 private:
  // Register ourself into the routing database. We need to be registered
  // into the routing database in order to start receiving messages.
  bool RegisterRoute();

  // Method that is called when a message is received.
  void OnMessageReceived(const MessageParts& message_parts);

  // The message processing entry point.
  void ProcessMessage(const ruby::protocol::RubyMessage& message);

  // Process the Announce message, which informs to the external world that
  // a service is beign hosted bythe service node.
  void Announce(const std::string& sender, const std::string& message);

  // Process query messages, which is used to check if the service node is
  // hosting a particular service.
  void QueryService(const std::string& message);
  
  // Converts a error code to a human readable message.
  // Returns an empty string if error_code is NODE_CONTROL_NO_ERROR
  std::string ErrorCodeToString(ProcessingError error_code);

  // Sends an error message to the message sender.
  void ReportError(ProcessingError error_code);

  zmq::Context* context_;
  MessageRouter* message_router_;
  ServicesDatabase* services_db_;

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