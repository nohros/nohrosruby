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

Message::Message(int buffer_size)
  : size_(buffer_size) {
  DCHECK(buffer_size > 0);
  data_ = new char[buffer_size];
}

Message::Message(char* data, int size)
  : data_(data),
    size_(size) {
}

Message::~Message() {
  delete[] data_;
  data_ = NULL;
}

}  // namespace zmq