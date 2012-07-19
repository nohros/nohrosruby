// Copyright (c) 2010 Nohros Systems Inc. All rights reserved
// Use of this source code is governed by BSD-style license that can be found
// in the LICENCE file.

// Should be included first to avoid conflicts with the "windows.h" indirectly
// included by the "message.h"
#include <zmq.h>

#include "node/zeromq/message.h"

#include <base/logging.h>

#include "node/zeromq/zmq_structs.h"

namespace zmq {

Message::Message()
  : data_(NULL) {
  message_ = new zmq_msg_t();
  zmq_msg_init(message_);
}

Message::Message(int size) {
  DCHECK(size > 0);
  data_ = new char[size];
  message_ = new zmq_msg_t();
  AddRef(); // Released in ReleaseMessage()
  zmq_msg_init_data(message_, data_, size, ReleaseMessage, this);
}

Message::~Message() {
  delete[] data_;
  delete message_;
  data_ = NULL;
  zmq_msg_close(message_);
}

size_t Message::size() {
  return zmq_msg_size(message_);
}

void* Message::mutable_data() {
  return zmq_msg_data(message_);
}

// static
void Message::ReleaseMessage(void* data, void* hint) {
  static_cast<Message*>(hint)->Release();
}

}  // namespace zmq