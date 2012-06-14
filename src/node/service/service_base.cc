// Copyright (c) 2012 Nohros Inc. All rights reserved.
// Use of this source code is governed by a BSD-style license that can be
// found in the LICENSE file.

#include "node/service/service_base.h"

#include <vector>
#include <string>

#include <base/memory/scoped_ptr.h>
#include <base/string_util.h>
#include <base/logging.h>

#include "node/service//service_logging.h"

namespace node {

ServiceBase* g_service = NULL;

ServiceBase::ServiceBase(const char* service_name)
  : service_name_(service_name),
    service_status_handle_(0),
    stop_event_(false, false),
    stop_wait_handle_(0) {

  // set up the initial service state
  service_status_.dwServiceType = SERVICE_WIN32_OWN_PROCESS;
  service_status_.dwCurrentState = SERVICE_START_PENDING;
  service_status_.dwControlsAccepted = SERVICE_ACCEPT_PAUSE_CONTINUE
    | SERVICE_ACCEPT_SHUTDOWN | SERVICE_ACCEPT_STOP;
  service_status_.dwWin32ExitCode = NO_ERROR;
  service_status_.dwServiceSpecificExitCode = 0;
  service_status_.dwCheckPoint = 0;
  service_status_.dwWaitHint = 0;

  DCHECK(!g_service);
  g_service = this;
}

void ServiceBase::Run() {
  SERVICE_TABLE_ENTRY entry[] = {
    { "", ServiceMainCallback }, { NULL, NULL }
  };

  BOOL ok = StartServiceCtrlDispatcher(entry);
  if (!ok) {
    LOG_SERVICE_START(ERROR, service_name.get());
    return;
  }

  VLOG(1) << "ServiceBase process launched";
}

// static
VOID WINAPI ServiceBase::ServiceMainCallback(DWORD argc, LPSTR *argv) {
  const char* service_name = g_service->service_name().c_str();
  SERVICE_STATUS_HANDLE service_status_handle =
    RegisterServiceCtrlHandlerEx(service_name, ServiceCommandCallback,
      static_cast<LPVOID>(g_service));

  if (service_status_handle == 0) {
    LOG_SERVICE_START(ERROR, service_name);

    VLOG(1) << "The control handler for the service "
            << service_name
            << " fails to be registered with error code "
            << logging::GetLastSystemErrorCode();
    return;
  }

  VLOG(1) << "Control handler has been registered";

  g_service->service_status_handle_ = service_status_handle;
  if (g_service->SetServiceState(SERVICE_START_PENDING)) {
    std::vector<std::string> arguments;
    for (DWORD i = 0; i < argc; i++) {
      arguments.push_back(std::string(argv[i]));
    }
    g_service->ServiceQueuedMainCallback(arguments);
  }
}

void ServiceBase::ServiceQueuedMainCallback(
  const std::vector<std::string>& arguments) {

  // Create a waitable event and register a waiter on then, allowing the
  // thread that is running this method to terminate without terminating the
  // service. The created event should be signaled when the service is
  // stopping.
  BOOL ok = RegisterWaitForSingleObject(&stop_wait_handle_, stop_event_.handle(),
    ServiceStopCallback, static_cast<PVOID>(this), INFINITE,
    WT_EXECUTEDEFAULT | WT_EXECUTEONLYONCE);

  if (!ok) {
    SetServiceState(SERVICE_STOPPED);
    return;
  }

  service_status_.dwCheckPoint = 0;
  service_status_.dwWaitHint = 0;
  if (!SetServiceState(SERVICE_RUNNING)) {
    LOG_SERVICE_START(ERROR, service_name_.c_str());
    VLOG(1)
      << "Failed to set the state of the service to RUNNING. Error code: "
      << logging::GetLastSystemErrorCode();
    
    // Try to put the service into the STOPPED state, if this calls fail
    // there are not we can do.
    SetServiceState(SERVICE_STOPPED);
    return;
  }

  OnStart(arguments);
}

// static
DWORD WINAPI ServiceBase::ServiceCommandCallback(DWORD command, DWORD event_type,
  LPVOID event_data, LPVOID context) {
  DCHECK(context);
  ServiceBase* service = static_cast<ServiceBase*>(context);
  switch (command) {
    case SERVICE_CONTROL_STOP:
      service->Terminate();
  }
  return NO_ERROR;
}

// static
VOID CALLBACK ServiceBase::ServiceStopCallback(PVOID context,
  BOOLEAN time_or_wait_fired) {
  DCHECK(context);

  ServiceBase* service = static_cast<ServiceBase*>(context);
  UnregisterWait(service->stop_wait_handle_);

  service->SetServiceState(SERVICE_STOPPED);
}

bool ServiceBase::SetServiceState(DWORD service_state) {
  service_status_.dwCurrentState = service_state;
  BOOL status_was_set = SetServiceStatus(service_status_handle_,
    &service_status_);

  if (!status_was_set) {
    LOG_SERVICE_START(ERROR, service_name.get());

    VLOG(1) << "The status of the service " << service_name_.c_str()
            << " fails to be set with error code: "
            << logging::GetLastSystemErrorCode();
    return false;
  }
  return true;
}

void ServiceBase::Terminate() {
  OnStop();
  stop_event_.Signal();
}

void ServiceBase::Teardown() {
}

ServiceBase::~ServiceBase() {
  Teardown();
  g_service = NULL;
}

}  // namespace node