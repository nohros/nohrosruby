// Copyright (c) 2010 Nohros Systems Inc. All rights reserved
// Use of this source code is governed by BSD-style license that can be found
// in the LICENCE file.

#ifndef NODE_ZEROMQ_CONTEXT_H_
#define NODE_ZEROMQ_CONTEXT_H_

extern "C" {
#include <zmq.h>
}

#include <vector>

#include <base/basictypes.h>
#include <base/memory/scoped_ptr.h>

#include "node/zeromq/basictypes.h"

namespace zmq {
class Socket;

class Context {
 friend class Socket;

 public:
  // Creates a nes instance of the Context class. The |io_threads| argument
  // specifies the size of the zeromq thread pool to handle I/O operations. If
  // your application is using only the inproc transport for messaging you may
  // set this to zero, otherwise set it to at least one.
  explicit Context(int io_threads);

  ~Context();

  // Create a zeromq socket. The CreateSocket function shall create a zeromq
  // socket within the current context and return an opaque handle to the newly
  // created socket. The type argument specifies the socket type, which
  // determines the semantics of communication over the socket. The newly
  // created socket is initially unbound, and not associated with any endpoints.
  // In order to establish a message flow a socket must first be connected to
  // at least one endpoint with Socket::Connect(), or at least one endpoint must
  // be created for accepting incoming connections with Socket::Bind()
  int CreateSocket(SocketType type, Socket* socket);

 protected:
  // Be carefull with this, it's probaly useful for using the C API
  // toghether with an existing C++ api. Normally oyu should never need
  // to use this.
  inline void* raw_zmq_context() {
    return zmq_context_.get();
  }

 private:
  // Initialize the current context.
  inline void Init(int io_threads);
  
  // The RAW zeromq context.
  scoped_ptr<void> zmq_context_;
};

}  // namespace zmq

#endif  // NODE_ZEROMQ_CONTEXT_H_