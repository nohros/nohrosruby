// Copyright (c) 2010 Nohros Systems Inc. All rights reserved
// Use of this source code is governed by BSD-style license that can be found
// in the LICENCE file.

// "socket.h" indirectly includes "windows.h" that includes "winsock.h" and
// "zmq.h" uses "winsock2.h" which redefines most of the structures defined in
// "winsock.h" so we need to include "winsock2.h" before any inclusion of
// "windows.h"
#include <winsock2.h>

#include "node/zeromq/socket.h"

#include "node/zeromq/basictypes.h"

namespace zmq {

Socket::Socket(scoped_refptr<Context::SocketRef> ref)
  : ref_(ref) {
}

Socket::~Socket() {
  Close();
}

void Socket::Close() {
  ref_->Close();
}

bool Socket::Connect(const char* endpoint) {
  if (!is_valid()) {
    return false;
  }
  return CheckError(zmq_connect(socket, endpoint)) == ZMQ_OK;
}

int Socket::CheckError(int err) {
  // Don't add DCHECKs herem OnZeromqError() already has them.
  if (is_valid()) {
    return ref_->context()->OnZeromqError(err, this);
  }
  return err;
}

}  // namespace zmq