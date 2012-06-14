// Copyright (c) 2010 Nohros Systems Inc. All rights reserved
// Use of this source code is governed by BSD-style license that can be found
// in the LICENCE file.

#include "node/zeromq/message.h"

#include <base/logging.h>

namespace zmq {

Message::Message()
  : data_(NULL),
    size_(0) {
}

Message::Message(int size)
  : size_(size) {
  DCHECK(size > 0);
  AddRef();  // Released in Release()
  data_ = new char[size];
  zmq_msg_init_data(&message_, data_, size, ReleaseMessage, this);
}

Message::Message(char* data, int size)
  : data_(data),
    size_(size) {
}

Message::~Message() {
  delete[] data_;
  data_ = NULL;
  zmq_msg_close(&message_);
}

// static
void Message::ReleaseMessage(void* data, void* hint) {
  static_cast<Message*>(hint)->Release();
}

WrappedMessage::WrappedMessage(const char* data, int size)
  : Message(const_cast<char*>(data), size) {
}

WrappedMessage::~WrappedMessage() {
  data_ = NULL;
  zmq_msg_close(message());
}

}  // namespace zmq