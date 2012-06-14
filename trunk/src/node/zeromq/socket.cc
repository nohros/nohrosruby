// Copyright (c) 2010 Nohros Systems Inc. All rights reserved
// Use of this source code is governed by BSD-style license that can be found
// in the LICENCE file.

#define ZMQ_OK 0

// Should be included first to avoid conflicts with the "windows.h" indirectly
// included by the "socket.h"
#include <zmq.h>

#include "node/zeromq/socket.h"

#include "node/zeromq/message.h"

namespace zmq {

Socket::Socket(scoped_refptr<Context::SocketRef> ref)
  : ref_(ref) {
}

Socket::~Socket() {
  Close();
}

bool Socket::Bind(const char* endpoint) {
  if (!is_valid()) {
    return false;
  }
  return CheckError(zmq_bind(ref_->socket(), endpoint)) == ZMQ_OK;
}

void Socket::Close() {
  ref_->Close();
}

bool Socket::Connect(const char* endpoint) {
  if (!is_valid()) {
    return false;
  }
  return CheckError(zmq_connect(ref_->socket(), endpoint)) == ZMQ_OK;
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

int Socket::CheckError(int err) {
  // Don't add DCHECKs here OnZeromqError() already has them.
  if (is_valid()) {
    return ref_->context()->OnZeromqError(err, this);
  }
  return err;
}

}  // namespace zmq