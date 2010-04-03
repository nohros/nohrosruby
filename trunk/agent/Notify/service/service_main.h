// Copyright (c) 2009 Nohros Systems Inc.
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

#define _WIN32_WINNT	0x0500


#ifndef AGENT_SERVICE_SERVICE_MAIN_H__
#define AGENT_SERVICE_SERVICE_MAIN_H__

#include <windows.h>
#include "third_party/chrome/logging.h"
#include "common/settings.h"

#define NOTIFY_SERVICE_START_LISTENER 0x80

SERVICE_STATUS_HANDLE hServiceStatus;
SERVICE_STATUS ssStatus;

HANDLE g_hSyncEvent;
HANDLE g_hWaitHandle;

VOID WINAPI NotifyServiceMain(DWORD, LPTSTR*);

/* 
 * Handle the service control requests.
 *
 * Author: Neylor Ohmaly Rodrigues e Silva - 2009-10-14
 *
 * History:
 *		2009-10-14 - Neylor - Release
 */
DWORD WINAPI NotifyHandleEx(DWORD, DWORD, LPVOID, LPVOID);

/* 
 * Performs cleanup and finish the service stop process
 *
 * Author: Neylor Ohmaly Rodrigues e Silva - 2009-10-15
 *
 * History:
 *		2009-10-15 - Neylor - Release
 */
VOID CALLBACK NotifyCleaner(PVOID, BOOLEAN);

#endif // RUBY_AGENT_SERVICE_SERVICE_MAIN_H__