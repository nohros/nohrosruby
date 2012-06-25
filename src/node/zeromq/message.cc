// Copyright (c) 2010 Nohros Systems Inc. All rights reserved
// Use of this source code is governed by BSD-style license that can be found
// in the LICENCE file.

#include "node/zeromq/message.h"

#include <base/logging.h>

namespace zmq {

Message::Message()
  : data_(NULL) {
  zmq_msg_init(&message_);
}

Message::Message(int size) {
  DCHECK(size > 0);
  data_ = new char[size];
  AddRef(); // Released in ReleaseMessage()
  zmq_msg_init_data(&message_, data_, size, ReleaseMessage, this);
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

}  // namespace zmq