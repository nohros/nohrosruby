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

#include "agent/service/service_main.h"

#include "listener.h"

/* Changes history:
 *		2009-10-23 - nohros - Release
 */
int main(int argc, char* argv[])
{
	SERVICE_TABLE_ENTRY dispatcher[] = { { TEXT("NotifyService"), NotifyServiceMain },  { NULL, NULL } };

	// global  synchronization event
	g_hSyncEvent = CreateEvent(NULL, FALSE, FALSE, L"NotifyGlobalEvent");
	if(g_hSyncEvent == NULL)
	{
		Logger::instance()->log( NNMSG_SERVICE_CREATEEVENT );
		return 0;
	}

#ifdef _DEBUG
	DebugBreak();
#endif

	TRACE( L"Starting the service control dispatcher thread" )

	// Create the service control dispatcher thread
	if(!StartServiceCtrlDispatcher( dispatcher ))
	{
		Logger::instance()->log( NNMSG_SERVICE_CTRLDISPATCHER );
	}

	return 0;
}

/* Changes history:
 *		2009-10-23 - nohros - Release
 */
VOID WINAPI NotifyServiceMain(DWORD dwArgc, LPTSTR* lpszArgv)
{	
	DWORD dwWait = -1;

	TRACE( L"Registering the control handler" )

	// Register the control handle
	hServiceStatus = RegisterServiceCtrlHandlerEx(lpszArgv[0], NotifyHandleEx, (LPVOID)NULL);

	if( hServiceStatus == (SERVICE_STATUS_HANDLE)0 )
	{
		Logger::instance()->log( GetLastError() );
		return;
	}

	// Set the service status to SERVICE_RUNNING
	ssStatus.dwServiceType = SERVICE_WIN32_OWN_PROCESS;
	ssStatus.dwCurrentState = SERVICE_RUNNING;
	ssStatus.dwControlsAccepted = SERVICE_ACCEPT_SHUTDOWN | SERVICE_ACCEPT_STOP | SERVICE_ACCEPT_PAUSE_CONTINUE;
	ssStatus.dwWin32ExitCode = NO_ERROR;
	ssStatus.dwServiceSpecificExitCode = 0;
	ssStatus.dwCheckPoint = 0;
	ssStatus.dwWaitHint = 0;

	if(!SetServiceStatus(hServiceStatus, &ssStatus))
	{
		Logger::instance()->log( GetLastError() );
	}

	TRACE( L"Service started" )

	Listener::instance()->startup();

	// Terminates this thread and let the NotifyCleaner method
	// to do your job.
	RegisterWaitForSingleObject(
		&g_hWaitHandle,
		g_hSyncEvent,
		NotifyCleaner,
		(PVOID)NULL,
		(dwWait == WAIT_OBJECT_0) ? 0 : INFINITE, /* If the event object is signaled, we need to call the cleanup routine immediatly by set the timeout to 0 */
		WT_EXECUTEDEFAULT | WT_EXECUTEONLYONCE);

	TRACE( L"Service main thread has ended" )
}

/* Changes history:
 *		2009-10-23 - nohros - Release
 */
DWORD WINAPI NotifyHandleEx(DWORD dwControl, DWORD dwEventType, LPVOID lpEventData, LPVOID lpContext)
{
	switch(dwControl)
	{
		case SERVICE_CONTROL_PAUSE:
			TRACE( L"The service has been received a pause control code" )
			ssStatus.dwCurrentState = SERVICE_PAUSED;
			break;

		case SERVICE_CONTROL_CONTINUE:
			TRACE( L"The service has been received a continue control code" )
			ssStatus.dwCurrentState = SERVICE_RUNNING;
			break;

		case SERVICE_CONTROL_STOP:
			TRACE( L"The service has been received a stop control code" )
			ssStatus.dwCheckPoint = 0;
			ssStatus.dwWaitHint = 3000;
			ssStatus.dwWin32ExitCode = 0;
			ssStatus.dwCurrentState = SERVICE_STOP_PENDING;

			// performs the cleanup
			SetEvent(g_hSyncEvent);
			break;

		case SERVICE_CONTROL_SHUTDOWN:
			TRACE( L"The service has been received a shutdown control code" )
			break;

		case NOTIFY_SERVICE_START_LISTENER:
			TRACE(L"The service has been received a NOTIFY_SERVICE_START_LISTENER control code." )
			ssStatus.dwCurrentState = SERVICE_RUNNING;

			Listener::instance()->startup();
			break;

		default:
			return ERROR_CALL_NOT_IMPLEMENTED;
	}


	// Send current status
	if(!SetServiceStatus(hServiceStatus, &ssStatus))
	{
		Logger::instance()->log( GetLastError() );
	}
	return NO_ERROR;
}

/* Changes history:
 *		2009-10-23 - nohros - Release
 */
VOID CALLBACK NotifyCleaner(PVOID lpParameter, BOOLEAN timedOut)
{
	UnregisterWaitEx(g_hWaitHandle,
		NULL // we cannot make a blocking call within a callback function
		);

	// dispose the classes
	// order is important
	Listener::instance()->~Listener();

	// must be released before the Configuration object
	// and after all other objects
	Logger::instance()->~Logger();

	// must be the last disposed object
	Configuration::instance()->~Configuration();

	ssStatus.dwWin32ExitCode = 0;
	ssStatus.dwCurrentState = SERVICE_STOPPED;
	SetServiceStatus( hServiceStatus, &ssStatus );

	CloseHandle(g_hSyncEvent);
}