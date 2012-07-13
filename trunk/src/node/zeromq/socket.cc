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

bool Socket::Bind(const std::string& endpoint) {
  return Bind(endpoint.c_str());
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

bool Socket::Send(Message* message, int size, SocketFlags flags) {
  return CheckError(
    zmq_send(ref_->socket(), message->message(), flags)) == ZMQ_OK;
}

std::vector<scoped_refptr<Message>> Socket::Receive(SocketFlags flags) {
  bool has_more = true;
  size_t more_size;
  std::vector<scoped_refptr<Message>> message_parts;
  do {
    // Get the next message part from socket.
    Message* message = new Message();
    if (CheckError(
      zmq_recv(ref_->socket(), message->message(), flags)) != ZMQ_OK) {
      break;
    }
    message_parts.push_back(message);

    // Check if the message is a multipart message.
    int err = zmq_getsockopt(ref_->socket(), ZMQ_RCVMORE, &has_more, &more_size);
    has_more = (CheckError(err) == ZMQ_OK) && has_more;
  } while (has_more);
  return message_parts;
}

int Socket::CheckError(int err) {
  // Don't add DCHECKs here OnZeromqError() already has them.
  if (err != ZMQ_OK && is_valid()) {
    return ref_->context()->OnZeromqError(err, this);
  }
  return err;
}

}  // namespace zmq