// Copyright (c) 2010 Nohros Systems Inc. All rights reserved
// Use of this source code is governed by BSD-style license that can be found
// in the LICENCE file.

#ifndef NODE_ZEROMQ_CONTEXT_H_
#define NODE_ZEROMQ_CONTEXT_H_

#include <set>

#include <base/basictypes.h>
#include <base/memory/scoped_ptr.h>
#include <base/memory/ref_counted.h>

namespace zmq {

// Possible values for type in CreateSocket(). These should match the values
// in zmq.h
enum SocketType {
  kExclusivePair = 0,
  kPublisher = 1,
  kSubscriber = 2,
  kRequest = 3,
  kReply = 4,
  kDealer = 5,
  kRouter = 6
};

class Socket;
class Context;

// Error delegate defines the interface to implement error handling and
// recovery for zeromq operations. This allows the rest of the classes to
// return true or false while the actual error code and failed socket are
// delivered using the OnError callback.
//
// The tipical usage is to centralize the code designed to handle low-level IO
// errors.
class ErrorDelegate : public base::RefCountedThreadSafe<ErrorDelegate> {
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
  virtual int OnError(int error, Context* context, Socket* socket) = 0;

 protected:
  friend class base::RefCountedThreadSafe<ErrorDelegate>;

  virtual ~ErrorDelegate();
};

class Context {
 private:
  class SocketRef; // Forward declaration, see real one below.

 public:
  // Creates a nes instance of the Context class. The context should be
  // initialized by calling Open([io_threads]). Any opened sockets
  // will be closed when this context is deleted.
  Context();
  ~Context();

  // Sets the objeta that will handle errors. Recommended that it shoud be set
  // before callind Open(). If not set, the default is to ignore errors on
  // release and assert on debug builds.
  void set_error_delegate(ErrorDelegate* delegate) {
    error_delegate_ = delegate;
  }

  // Initialization ----------------------------------------------------------
  
  // Initializes the ZeroMQ context for the given number of io threads,
  // returning true uf the context is successfully created. The |io_threads|
  // argument specifies the size of the zeromq thread pool to handle I/O
  // operations. If your application is using only the inproc transport for
  // messaging you may set this to zero, otherwise set it to at least one.
  bool Open(int io_threads);

  // Returns true if the context has been sucessfully opened.
  bool is_open() const { return !!zmq_context_; }

  // Closes the context. This is automatically performed on destruction for
  // you, but this allows you to performs a deterministic close. You must
  // not call any other functions after closing it. It is permissible to call
  // Close on an uninitialized or already-closed context.
  void Close();

  // Creates a zeromq socket. The CreateSocket function shall create a zeromq
  // socket within the current context and return an ref counted pointer to the
  // newly created socket.
  //
  // The type argument specifies the socket type, which determines the
  // semantics of communication over the socket. The newly created socket is
  // initially unbound, and not associated with any endpoints. In order to
  // establish a message flow a socket must first be connected to at least one
  // endpoint with Socket::Connect(), or at least one endpoint must be created
  // for accepting incoming connections with Socket::Bind().
  //
  // If an socket could not be created, because of an error, an invalid, inert
  // SocketRef is returned (and the code will crash on debug. The caller must
  // deal with this eventuallity, by correctly handling the return of an inert
  // socket.
  scoped_refptr<SocketRef> CreateSocket(SocketType type);

  // Errors -----------------------------------------------------------------

  // Returns the error code associated with the last zeromq operation.
  int GetErrorCode() const;

  // Returns a pointer to a statically allocated string associated with the
  // last zeromq operation.
  const char* GetErrorMessage() const;

 private:
  // Sockets accesses SocketRef which we don't want to expose to everybody.
  // (they should go through Statement).
  friend class Socket;

  // A SocketRef is refcounted wrapper around a zeromq socket pointer.
  // Refcounting allow us to give these sockets out to zmq::Socket
  // objects while also optionally maintaining a refptr to these objetcs.
  //
  // A socket ref can be valid, in which case it can be used, or invalid to
  // indicate that the socket hasn't been created yet, has an error, or has
  // been destroyed.
  //
  // The context may revoke a SocketRef in some error cases, so callers
  // should always check the validity before using.
  class SocketRef : public base::RefCountedThreadSafe<SocketRef> {
   public:
    // Default constructor initializes to an invalid socket.
    SocketRef();
    SocketRef(Context* context, void* socket);

    // When true socket could be used.
    bool is_valid() const { return !!socket_; }

    // If we've been linked to a context, this will be NULL. Guaranteed
    // non-NULL when is_valid().
    Context* context() const { return context_; }

    // Returns the zeromq socket if any. If the socket is not active, this will
    // return NULL.
    void* socket() const { return socket_; }
    
    // Destroys the socket and marks it NULL. The socket will no longer be
    // active.
    void Close();

   private:
    friend class base::RefCountedThreadSafe<SocketRef>;

    ~SocketRef();

    Context* context_;
    void* socket_;

    DISALLOW_COPY_AND_ASSIGN(SocketRef);
  };
  friend class SocketRef;

  // Called by a SocketRef when it's beign created or destroyed. See
  // open_sockets_ below.
  void SocketRefCreated(SocketRef* ref);
  void SocketRefDeleted(SocketRef* ref);

  // Called by Socket objetcs when a zeromq function returns an error. The
  // return value is the error code reflected back to the client code.
  int OnZeromqError(int err, Socket* socket);

  void* zmq_context_;

  // A list of all SocketReds we've given out. Each ref must register with
  // us when it's created or destroyed. This allows us to potentially close
  // any open sockets on closing/destruction.
  typedef std::set<SocketRef*> SocketRefSet;
  SocketRefSet open_sockets_;

  // This object handles errors resulting from all forms of executing zeromq
  // commands. It can be null which means default handling.
  scoped_refptr<ErrorDelegate> error_delegate_;

  DISALLOW_COPY_AND_ASSIGN(Context);
};

}  // namespace zmq

#endif  // NODE_ZEROMQ_CONTEXT_H_