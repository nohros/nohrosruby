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
#include <base/memory/ref_counted.h>

#include "node/zeromq/basictypes.h"

namespace zmq {
class Socket;
class Context;

// Error delegate defines the interface to implement error handling and
// recovery for zeromq operations. This allows the rest of the classes to
// return true or false while the actual error code and failed socket are
// delivered using the OnError callback.
//
// The tipical usage is to centralize the code designed to handle low-level IO
// errors.
class ErrorDelegate : public base::RefCounted<ErrorDelegate> {
 public:
  ErrorDelegate();

  // |error| is an zeromq result code as seen in zeromq\include\zmq.h
  // |context| is zeromq context where the error happened and |socket| is
  // the socket that trigger the error. Do not store these pointers.
  //
  // |socket| MAY BE NULL if there is no socket causing the problem (i.e on
  // initialization).
  //
  // If error condition has been fixed an the original operation successfully
  // re-tried then rturning ZMQ_OK is appropiate; otherwise is recomended
  // that you return the original |error| or the appropiate error code.
  virtual int OnError(int error, Context* context, Socket* socket);
};

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
  scoped_refptr<ScoketRef> CreateSocket(SocketType type);

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