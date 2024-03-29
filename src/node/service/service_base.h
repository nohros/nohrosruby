// Copyright (c) 2012 Nohros Inc. All rights reserved.
// Use of this source code is governed by a BSD-style license that can be
// found in the LICENSE file.

#ifndef NODE_SERVICE_SERVICE_BASE_H_
#define NODE_SERVICE_SERVICE_BASE_H_

#include <string>
#include <vector>
#include <windows.h>

#include <base/basictypes.h>
#include <base/synchronization/waitable_event.h>

class CommandLine;

namespace node {

// Provides a base class for a service that will exists as part of a service
// application. Each service application should instantiate only one
// ServiceBase class.
class ServiceBase {

 public:
  explicit ServiceBase(const wchar_t* service_name);

  virtual ~ServiceBase();

  // Register the executable for a service with the ServiceBase Control
  // Manager (SCM). THis method shoul be called in the main() funtion of
  // the service executable to register the service with the ServiceBase Control
  // Manager. After you call Run(const ServiceBase& service), the ServiceBase Control
  // Manager issues a Start command, which results in a call to the OnStart
  // method in the service. The service is not started until the Start command
  // is executed.
  void Start();

  // Gets the service's name as seen by the SCM.
  const std::wstring& service_name() const { return service_name_; };

 protected:
  // Executed when a Start command is sent to the service by the Service
  // Control Manager (SCM). OnStart is the method in which you specify the
  // behavior of the service. OnStart can take arguments as a way to pass
  // data, but this usage is rare.
  virtual bool OnStart(const std::vector<std::wstring>& arguments) = 0;

  // Executed when a Stop command is sent to the service by the Service Control
  // Manager (SCM).
  virtual void OnStop() = 0;

 private:
  static VOID WINAPI ServiceMainCallback(DWORD argc, LPWSTR *argv);
  static DWORD WINAPI ServiceCommandCallback(DWORD command, DWORD event_type,
    LPVOID event_data, LPVOID context);

  // The method that will be executed when the service receives a Stop command
  // from the service control manager.
  static VOID CALLBACK ServiceStopCallback(PVOID context,
    BOOLEAN time_or_wait_fired);

  // Directs a wait thread in the thread pool to wait for the service to
  // terminate and finilize the service start process by putting it in the
  // Running state.
  void ServiceQueuedMainCallback(const std::vector<std::wstring>& arguments);

  // Updates the service state of the ServiceBase Control Manager's status
  // information for the service.
  bool SetServiceState(DWORD service_state);

  // Terminate forces the service process to stop.
  void Terminate();

  void Teardown();

  // The name of the service as seen by th ServiceBase Control Manager (SCM).
  std::wstring service_name_;

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

  DISALLOW_COPY_AND_ASSIGN(ServiceBase);
};

extern ServiceBase* g_service;

}  // namespace node
#endif  // NODE_SERVICE_SERVICE_BASE_H_
