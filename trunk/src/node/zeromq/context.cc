// Copyright (c) 2010 Nohros Systems Inc. All rights reserved
// Use of this source code is governed by BSD-style license that can be found
// in the LICENCE file.

#include "node/zeromq/context.h"

#include <base/logging.h>

#include "node/zeromq/socket.h"

namespace zmq {

Context::Context(int io_threads)
  : zmq_context_(NULL) {
  Init(io_threads);
}

// static
int Context::CreateSocket(SocketType type, Socket* socket) {
  scoped_ptr<Socket> local_socket(new Socket(type));
  local_socket.get()->zmq_socket_ = zmq_socket(zmq_context_, type);
  int error = local_socket.get()->Init();
  return error;
}

void Context::Init(int io_threads) {
  zmq_context_.reset(zmq_init(io_threads));
  if (!zmq_context_.get()) {
    LOG(ERROR) << "The zeromq context fails to initialize.";
    return;
  }
}

}  // namespace zmq