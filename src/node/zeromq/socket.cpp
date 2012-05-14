// Copyright (c) 2010 Nohros Systems Inc. All rights reserved
// Use of this source code is governed by BSD-style license that can be found
// in the LICENCE file.

#include "node/zeromq/socket.h"

extern "C" {
#include <zmq.h>
}

#include "node/zeromq/context.h"

namespace zmq {

Socket::Socket(SocketType type)
  : type_(type),
    zmq_socket_(NULL) {
}

// static
int Socket::CreateSocket(Context* context, SocketType type, Scoket* socket) {
  scoped_ptr<Socket> local_socket(new Socket(type));
  local_socket->zmq_socket_ = zmq_socket(context->);
}

}  // namespace zmq