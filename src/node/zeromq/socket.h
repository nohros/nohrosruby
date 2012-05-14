// Copyright (c) 2010 Nohros Systems Inc. All rights reserved
// Use of this source code is governed by BSD-style license that can be found
// in the LICENCE file.

#ifndef NODE_ZEROMQ_SOCKET_H_
#endif NODE_ZEROMQ_SOCKET_H_

#include <base/memory/scoped_ptr.h>

#include "node/zeromq/basictypes.h"

namespace zmq {
class Context;

class Socket {
 public:
  Socket(SocketType type);

  ~Socket();

  // Gets the type of the socket.
  SocketType type() {
   return type_;
  }

 protected:
  // Create a zeromq socket. The CreateSocket function shall create a zeromq
  // socket within the current context and return an opaque handle to the newly
  // created socket. The type argument specifies the socket type, which
  // determines the semantics of communication over the socket. The newly
  // created socket is initially unbound, and not associated with any endpoints.
  // In order to establish a message flow a socket must first be connected to
  // at least one endpoint with Socket::Connect(), or at least one endpoint must
  // be created for accepting incoming connections with Socket::Bind()
  static int CreateSocket(Context* context, SocketType type);

 private:
   SocketType type_;

   // The RAW zeromq socket.
   scoped_ptr<void> zmq_socket_;
};

}  // namespace zmq

#endif  // NODE_ZEROMQ_SOCKET_H_