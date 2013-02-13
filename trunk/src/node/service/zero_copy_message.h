// Copyright (c) 2010 Nohros Systems Inc. All rights reserved
// Use of this source code is governed by BSD-style license that can be found
// in the LICENCE file.

#ifndef NODE_SERVICE_ZERO_COPY_MESSAGE_H_
#define NODE_SERVICE_ZERO_COPY_MESSAGE_H_

#include <base/memory/scoped_ptr.h>
#include <google/protobuf/io/zero_copy_stream_impl_lite.h>

#include "node/zeromq/message.h"

namespace node {

// A zero copy output implementation of zmq::Message.
class ZeroCopyMessage :
  public zmq::Message,
  public google::protobuf::io::ArrayOutputStream {

 public:
   ZeroCopyMessage(int size);
   ~ZeroCopyMessage();

 private:
};

}  // namespace node

#endif  // NODE_SERVICE_ZERO_COPY_MESSAGE_H_