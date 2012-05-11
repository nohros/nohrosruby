// Copyright (c) 2012 Nohros Inc. All rights reserved.
// Use of this source code is governed by a BSD-style license that can be
// found in the LICENSE file.

#ifndef NODE_SERVICE_SERVICE_H_
#define NODE_SERVICE_SERVICE_H_

#include <string>
#include <vector>
#include <windows.h>

#include <base/basictypes.h>
#include <base/synchronization/waitable_event.h>

namespace node {

class ServiceBase {

 public:
  explicit ServiceBase(const char* service_name);

  ~ServiceBase();

  // Register the executable for a service with the ServiceBase Control
  // Manager (SCM). THis method shoul be called in the main() funtion of
  // the service executable to register the service with the ServiceBase Control
  // Manager. After you call Run(const ServiceBase& service), the ServiceBase Control
  // Manager issues a Start command, which results in a call to the OnStart
  // method in the service. The service is not started until the Start command
  // is executed.
  void Run();

  // Executed when a Start command is sent to the service by the ServiceBase
  // Control Manager (SCM). OnStart is the method in which you specify the
  // behavior of the service. OnStart can take arugments as a way to pass
  // data, but this usage is rare.
  virtual void OnStart(const std::vector<std::string>& arguments) = 0;

  virtual void OnStop() = 0;

  // Gets the service's name as seen by the SCM.
  const std::string& service_name() const { return service_name_; };

 private:
  static VOID WINAPI ServiceMainCallback(DWORD argc, LPSTR *argv);
  static DWORD WINAPI ServiceCommandCallback(DWORD command, DWORD event_type,
    LPVOID event_data, LPVOID context);

  // The method that will be executed when the service receives a Stop command
  // from the service control manager.
  static VOID CALLBACK ServiceStopCallback(PVOID context,
    BOOLEAN time_or_wait_fired);

  // Directs a wait thread in the thread pool to wait for the service to
  // terminate and finilize the service start process by putting it in the
  // Running state.
  void ServiceQueuedMainCallback(const std::vector<std::string>& arguemnts);

  // Updates the service state of the ServiceBase Control Manager's status
  // information for the service.
  bool SetServiceState(DWORD service_state);

  // Terminate forces the service process to stop.
  void Terminate();

  void Teardown();

  // The name of the service as seen by th ServiceBase Control Manager (SCM).
  std::string service_name_;

  // A structure that contains status information for a service.
  SERVICE_STATUS service_status_;

  // A handle used to set the status of the service.
  SERVICE_STATUS_HANDLE service_status_handle_;

  // An event that is signaled when the service receives a Stop command from
  // the ServiceBase Control Manager(SCM).
  base::WaitableEvent stop_event_;

  // An handle that identifies the object that is waiting the |shutdown_event_|
  // to be signaled.
  HANDLE stop_wait_handle_;
};

extern ServiceBase* g_service;

}  // namespace node
#endif  // NODE_SERVICE_SERVICE_H_