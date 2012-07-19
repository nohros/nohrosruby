// Copyright (c) 2010 Nohros Systems Inc. All rights reserved
// Use of this source code is governed by BSD-style license that can be found
// in the LICENCE file.

#ifndef NODE_ZEROMQ_SOCKET_H_
#define NODE_ZEROMQ_SOCKET_H_

#include <vector>
#include <base/memory/ref_counted.h>

#include "node/zeromq/context.h"

namespace zmq {

// Possible values for flags in Send()/Receive(). These should match the values
// in zmq.h
enum SocketFlags {
  kNoFlags = 0,
  kNoBlock = 1,
  kSendMore = 2,
};

class Message;
typedef std::vector<scoped_refptr<Message>> MessageParts;

// Normal usage:
//   zmq::Socket socket(context_.CreateSocket(...))
//
//   If there are errors getting the socket, the socket will be inert; no
//   mutating or recv/send methods will work. If will need to check for validity,
//   use:
//   if (socket.is_valid()) {
//     return false;
//   }
//
// Socket method just return true to signal success. If you want to handle
// specific errors, install a error handler in the context object using
// set_error_delegate().
class Socket {
 public:
  explicit Socket(scoped_refptr<Context::SocketRef> ref);
  ~Socket();

  // Initializes this object with the given socket, which may or may not
  // be valid. Use is_valid() to check if it's OK.
  // void Assign(scoped_refptr<Context::SocketRef> ref);

  // Create an endpoint for accepting connections on the socket.
  // |endpoint| is a string consisting of two parts as follows:
  // [transport:address]. The [transport] part specifies the underlying
  // transport protocol to use. The meaning of [address] part is specific
  // to the underlying transport protocol selected.
  //
  // With the exception of SocketType::kPair sockets, a single socket may be
  // connected to multiple endpoints using Socket::Connect(), while
  // simultaneously accepting incoming connections from multiple endpoints
  // bound to the socket using Socket::Bind().
  bool Bind(const char* endpooint);
  bool Bind(const std::string& endpoint);

  // Destoys the socket. Any oustanding messages physically received from the
  // network but not yet received by the application with Socket::Receive()
  // shall be discarded. The behavior for discarding messages sent by the
  // application with Socket::Send() but not yet physically transfered to the
  // network depends on the value of the socket linger option.
  void Close();

  // Connect the socket to the endpoint specified by the |endpoint| argument.
  //
  // |endpoint| is a string consisting of two parts as follows:
  // [transport:address]. The [transport] part specifies the underlying
  // transport protocol to use. The meaning of [address] part is specific
  // to the underlying transport protocol selected.
  //
  // With the exception of SocketType::PAIR sockets, a single socket may be
  // connected to multiple endpoints using Socket::Connect(), while
  // simultaneously accepting incoming connections from multiple endpoints
  // bound to the socket using Socket::Bind().
  bool Connect(const char* endpoint);

  // Send a message on a socket. The Socket::Send() function shall we queue the
  // message referenced by the |message| argument to be sent to the socket.
  // |flags| is a combination of the flags defined below:
  //
  //  * ZMQ_NOBLOCK
  //     Specifies that the operation should be performed in non-bloking mode.
  //     If the message cannot be queued on the socket, the Socket::Send()
  //     function shall fail with error code EAGAIN.
  //
  //  * ZMQ_SNDMORE
  //     Specifies that the emssage beign sent is a multi-part, and that
  //     further message parts are follows.
  //
  // Refer to zeromq docs for a detailed description.
  bool Send(Message* message, int size, SocketFlags flags);

  // Receive all the parts os a message from a socket. The Socket::Receive()
  // function shall receive all the parts of a multi-part message from a socket
  // and store it in |pairs|. If there are no messages available the
  // Socket::Receive() function shall block until the request can be satisfied.
  // The flags argument is a combination of the flags defined below:
  //
  // If the message is not a multi-part message the |parts| vector will
  // contain only one element, which is the representiation of the the received
  // single-part message.
  //
  // * ZMQ_NOBLOCK
  //     Specifies that the operation should be performed in non-blocking mode.
  //     If there are no messages available on the specified socket, the
  //     Socket::Receive() function shall fail and |message| should contain no
  //     data.
  //
  // Refer to zeromq docs for a detailed description.
  bool Receive(MessageParts* parts, SocketFlags flags);

  // Returns true if the socket can be used.
  bool is_valid() const { return ref_->is_valid(); }

 private:
  // This is intended to check for errors and report them to the context
  // object. It takes a zeromq error code, and returns the same code.
  int CheckError(int err);

  // The actual zeromq socket. This pointer is guarantee non-NULL.
  scoped_refptr<Context::SocketRef> ref_;

  DISALLOW_COPY_AND_ASSIGN(Socket);
};

}  // namespace zmq

#endif  // NODE_ZEROMQ_SOCKET_H_