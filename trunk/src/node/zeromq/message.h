// Copyright (c) 2010 Nohros Systems Inc. All rights reserved
// Use of this source code is governed by BSD-style license that can be found
// in the LICENCE file.

#ifndef NODE_ZEROMQ_MESSAGE_H_
#define NODE_ZEROMQ_MESSAGE_H_

#include <base/memory/ref_counted.h>

namespace zmq {

// Messages are reference counted data buffers used as destination buffers for
// Read() operations, or as the source buffers for Write() operations.
//
// ---------------------------
// Ownership of Message:
// ---------------------------
//
// Altough Messages are RefcountedThreadSafe, they are not intended to be
// used as a shared buffer, not should they be used simultaneously accross
// threads. The fact that they are reference counted is an implementation
// detail for allowing them to outlive zeromq asynchrounous operations. In
// particular the Send() operation that could be effectivated at some unknown
// time.
//
// Instead, think of the underlying |char*| buffer contained by the Message as
// having exactly one owner at a time.
//
// Whenever you call an operation that takes a Message, ownership is implicity
// transferred to the called function.
//
//     ==> The Message data should NOT be manipulated, destroyed, or read after
//         it is passed to some function.
// 
// This usage contract is assumed by any API which takes a Message, even
// through it may not be explicity mentioned in the function's comments.
// 
// ---------------------------
// Motivation
// ---------------------------
//
// The motivation for transfering ownership to the called function is to make
// easier to work with the zeromq asynchronous operations.
//
// For istance, let's say under the hood our API called out to the zeromq
// asynchronous zmq_send() function. The successful invocation of the
// zmq_send() does not indicate that the message has been transmitted to the
// network, only that it has been queued on the socket and the zeromq has
// assumed responsability to for the buffer and hence the buffer it was reading
// from must remain alive. Using reference counting we can add a reference to
// the Message and make sure it is not destroyed until after send the operation
// has completed.
class Message : base::RefCountedThreadSafe<Message> {
 public:
  Message();
  explicit Message(int buffer_size);

  char* data() { return data_; }

  int size() const { return size_; }

 protected:
  friend class base::RefCountedThreadSafe<Message>;

  // Only allow derived classes to specify data_.
  // In all other cases, we own data_, and must delete it at destruction time.
  explicit Message(char* data, int size);

  virtual ~Message();

  char* data_;
  int size_;
};

}  // namespace zmq

#endif  // NODE_ZEROMQ_MESSAGE_H_