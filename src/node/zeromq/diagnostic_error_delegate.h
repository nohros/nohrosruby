// Copyright (c) 2010 Nohros Systems Inc. All rights reserved
// Use of this source code is governed by BSD-style license that can be found
// in the LICENCE file.

#ifndef NODE_SERVICE_DIAGNOSTIC_ERROR_DELEGATE_H_
#define NODE_SERVICE_DIAGNOSTIC_ERROR_DELEGATE_H_

namespace zmq {

// This class handles the exceptional zmq errors that might we encounter
// while running low-level zmq functions.
class DiagnosticErrorDelegate : public ErrorDelegate {
 public:

  virtual int OnError(int error, zmq::Context* context, zmq::Socket* socket) {
    LOG(ERROR) << "zmq error " << error
               << ", errno " << context->GetErrorCode()
               << ": " << context->GetErrorMessage();
    return error;
  }
};

}  // namespace zmq

#endif  // NODE_SERVICE_DIAGNOSTIC_ERROR_DELEGATE_H_