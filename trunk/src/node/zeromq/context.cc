// Copyright (c) 2010 Nohros Systems Inc. All rights reserved
// Use of this source code is governed by BSD-style license that can be found
// in the LICENCE file.

// "context.h" indirectly includes "windows.h" that includes "winsock.h" and
// "zmq.h" uses "winsock2.h" which redefines most of the structures defined in
// "winsock.h" so we need to include "winsock2.h" before any inclusion of
// "windows.h"
#include <winsock2.h>

#include "node/zeromq/context.h"

#include <zmq.h>
#include <base/logging.h>


namespace zmq {

ErrorDelegate::ErrorDelegate() {
}

ErrorDelegate::~ErrorDelegate() {
}

Context::SocketRef::SocketRef()
  : context_(NULL),
    socket_(NULL) {
}

Context::SocketRef::SocketRef(Context* context, void* socket)
  : context_(context),
    socket_(socket) {
  context_->SocketRefCreated(this);
}

Context::SocketRef::~SocketRef() {
  if (context_) {
    context_->SocketRefDeleted(this);
  }
  Close();
}

void Context::SocketRef::Close() {
  if (socket_) {
    zmq_close(socket_);
    socket_ = NULL;
  }
  context_ = NULL; // The context may be getting deleted.
}

Context::Context()
  : zmq_context_(NULL) {
}

Context::~Context() {
  Close();
}

bool Context::Open(int io_threads) {
  if (zmq_context_) {
    DLOG(FATAL) << "zmq::Context is already open.";
    return false;
  }

  zmq_context_ = zmq_init(io_threads);
  if (zmq_context_ == NULL) {
    OnZeromqError(*static_cast<int*>(zmq_context_), NULL);
    Close();
    return false;
  }
  return true;
}

void Context::Close() {
  // zmq_term() needs all open sockets to be finalized. Assert that the client
  // has relesead all socket.
  DCHECK(open_sockets_.empty());

  // Additionaly clear the opened sockets, because they contain weak references
  // to this context. This case can come up when error-handling code is hit
  // on production.

  if (zmq_context_) {
    zmq_term(zmq_context_);
    zmq_context_ = NULL;
  }
}

scoped_refptr<Context::SocketRef> Context::CreateSocket(SocketType socket_type) {
  if (!zmq_context_) {
    return new SocketRef(this, NULL); // return inactive socket.
  }

  void* socket = zmq_socket(zmq_context_, socket_type);
  if (socket == NULL) {
    DLOG(FATAL) << "Socket creation error " << GetErrorMessage();
    return new SocketRef(this, NULL);
  }
  return new SocketRef(this, socket);
}

int Context::GetErrorCode() const {
  return zmq_errno();
}

const char* Context::GetErrorMessage() const {
  return zmq_strerror(zmq_errno());
}

void Context::SocketRefCreated(SocketRef* ref) {
  DCHECK(open_sockets_.find(ref) == open_sockets_.end());
  open_sockets_.insert(ref);
}

void Context::SocketRefDeleted(SocketRef* ref) {
  for (SocketRefSet::iterator i = open_sockets_.begin();
    i != open_sockets_.end(); ++i) {
    (*i) ->Close();
  }
}

int Context::OnZeromqError(int err, Socket* socket) {
  if (error_delegate_.get()) {
    return error_delegate_->OnError(err, this, socket);
  }
  // The default handling is to assert on debug and to ignore on release.
  DLOG(FATAL) << GetErrorMessage();
  return err;
}

}  // namespace zmq