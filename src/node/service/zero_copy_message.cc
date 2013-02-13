// Copyright (c) 2010 Nohros Systems Inc. All rights reserved
// Use of this source code is governed by BSD-style license that can be found
// in the LICENCE file.

#include "node/service/zero_copy_message.h"

namespace node {
namespace gpb = ::google::protobuf;

ZeroCopyMessage::ZeroCopyMessage(int size)
  : zmq::Message(size),
    gpb::io::ArrayOutputStream(::zmq::Message::data_, size) {
}

ZeroCopyMessage::~ZeroCopyMessage() {
}

}  // namespace node