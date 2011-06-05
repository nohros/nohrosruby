// Copyright (c) 2010 Nohros Systems Inc.
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this 
// software and associated documentation files (the "Software"), to deal in the Software 
// without restriction, including without limitation the rights to use, copy, modify, merge, 
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons 
// to whom the Software is furnished to do so, subject to the following conditions:
// 	
// The above copyright notice and this permission notice shall be included in all copies or 
// substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, 
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR 
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE 
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR 
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
// DEALINGS IN THE SOFTWARE.
// ===========================================================================================

#ifndef CLIENT_SERVICE_SERVICE_MAIN_H__
#define CLIENT_SERVICE_SERVICE_MAIN_H__

#include <atlbase.h>

#include "third_party/chrome/base/basictypes.h"
#include "common/const_ruby.h"
#include "common/resource.h"

namespace ruby {

class ServiceModule
    : public CAtlServiceModuleT<ServiceModule, IDS_SERVICE_NAME> {
  public:
    typedef CAtlServiceModuleT<ServiceModule, IDS_SERVICE_NAME> Base;

    ServiceModule();
    ~ServiceModule();

    /**
     * Runs the entry point for the service
     */
    int Main(int show_cmd);

    /**
     * These methods are called by ATL and they must be public
     */
    void ServiceMain(DWORD argc, LPTSTR* argv) throw();
    //HRESULT PreMessageLoop(int show_cmd);
    //HRESULT PostMessageLoop();
    //HRESULT InitializeSecurity() throw();
    //HRESULT RegisterClassObjects(DWORD class_context, DWORD flags) throw();
    //HRESULT RevokeClassObjects() throw();
    //HRESULT RegisterServer(BOOL reg_typelib = FALSE, const CLSID* clsid = NULL) throw();
    //HRESULT UnrgisterServer(BOOL reg_typelib, const CLSID* clsid = NULL) throw();
    //LONG Unlock() throw();
    //void MonitorShutdown() throw();
    //HANDLE StartMonitor() throw();
    //static DWORD WINAPI MonitorProc(void* pv) throw();

  private:
    // Calls the service dispatcher to start the service.
    HRESULT Start(int show_cmd);

	HANDLE service_thread_; // The service thread provided by the SCM.

	DISALLOW_EVIL_CONSTRUCTORS(ServiceModule);
};

} // namespace ruby

#endif // CLIENT_SERVICE_SERVICE_MAIN_H__