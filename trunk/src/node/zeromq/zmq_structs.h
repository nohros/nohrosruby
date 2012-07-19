// Copyright (c) 2010 Nohros Systems Inc. All rights reserved
// Use of this source code is governed by BSD-style license that can be found
// in the LICENCE file.

#include <zmq.h>

// Complete declaration of the zmq structures. This header should be included
// only in implemetation files (.cc). This file exists to make possible forward
// declare the zmq structures. Forward declaring zmq structures is
// very important, because zmq C header has a lot of stuffs that may not
// be visible to the the users of the "message.h" header file.
namespace zmq {
  struct zmq_msg_t : public ::zmq_msg_t {};
}