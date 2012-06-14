// Copyright (c) 2010 Nohros Systems Inc. All rights reserved
// Use of this source code is governed by BSD-style license that can be found
// in the LICENCE file.

#ifndef NODE_ZEROMQ_BASICTYPES_H_
#define NODE_ZEROMQ_BASICTYPES_H_
#pragma once

#include <zmq.h>

namespace zmq {

#define ZMQ_OK 0

enum SocketType {
  ExclusivePair = ZMQ_PAIR,
  Publisher = ZMQ_PUB,
  Subscriber = ZMQ_SUB,
  Request = ZMQ_REQ,
  Reply = ZMQ_REP
};

}  // namespace zmq

#endif  // NODE_ZEROMQ_BASICTYPES_H_