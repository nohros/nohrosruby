// Copyright (c) 2010 Nohros Systems Inc. All rights reserved
// Use of this source code is governed by BSD-style license that can be found
// in the LICENCE file.

// "socket.h" indirectly includes "windows.h" that includes "winsock.h" and
// "zmq.h" uses "winsock2.h" which redefines most of the structures defined in
// "winsock.h" so we need to include "winsock2.h" before any inclusion of
// "windows.h"
#include <winsock2.h>

#include "node/zeromq/socket.h"

#include "node/zeromq/message.h"
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
  // Don't add DCHECKs here OnZeromqError() already has them.
  if (is_valid()) {
    return ref_->context()->OnZeromqError(err, this);
  }
  return err;
}

bool Socket::Send(Message* message, int size, int flags) {
  return CheckError(
    zmq_send(ref_->socket(), message->message(), flags)) == ZMQ_OK;
}

scoped_refptr<Message> Socket::Receive(int flags) {
  zmq_msg_t message;
  zmq_msg_init(&message);
  CheckError(zmq_recv(ref_->socket(), &message, flags));
  
  return new WrappedMessage(
    static_cast<char*>(zmq_msg_data(&message)), zmq_msg_size(&message));
}

}  // namespace zmq