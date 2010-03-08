#pragma region MIT License
/*
 * Nohros Notify
 * Copyright (c) 2009 Nohros Systems Inc, http://www.nohros.com
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy of this 
 * software and associated documentation files (the "Software"), to deal in the Software 
 * without restriction, including without limitation the rights to use, copy, modify, merge, 
 * publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons 
 * to whom the Software is furnished to do so, subject to the following conditions:
 * 	
 * The above copyright notice and this permission notice shall be included in all copies or 
 * substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, 
 * INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR 
 * PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE 
 * FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR 
 * OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
 * DEALINGS IN THE SOFTWARE.
 */

#pragma endregion

#include "../notify/config.h"
#include "../notify/logging.h"
#include "notify.h"

/* Changes history:
 *		2009-10-23 - nohros - Release
 */
int WINAPI WinMain(HINSTANCE hInstance, HINSTANCE hPrevInstance, LPSTR lpCmdLine, int nCmdShow)
{
	MSG msg;
	int x;				// initial horizontal position of the window
	int y;				// initial vertical position of the window
	int nWidth = 264;	// window width
	int nHeight = 139;	// window height
	BOOL bRet;

	// save the instance
	hinst = hInstance;

	// Register the window class
	WNDCLASSEX wcx;
	wcx.cbSize = sizeof(wcx);
	wcx.style = CS_NOCLOSE;
	wcx.lpfnWndProc = WndProc;
	wcx.cbClsExtra = 0;
	wcx.cbWndExtra = 0;
	wcx.hInstance = hInstance;
	wcx.hIcon = LoadIcon(NULL, IDI_APPLICATION);
	wcx.hCursor = LoadCursor(NULL, IDC_ARROW);
	wcx.hbrBackground = (HBRUSH)COLOR_WINDOW;
	wcx.lpszMenuName = NULL;
	wcx.lpszClassName = L"NS Ruby Notification";
	wcx.hIconSm = NULL;

	// Register the windows class
	if(!RegisterClassEx(&wcx)) {
		return 0;
	}

	// Compute the x and y positions
	RECT lpRect;
	HWND tray = FindWindow(TEXT("Shell_TrayWnd"), NULL);
	if(tray != NULL)
	{
		if(GetWindowRect(tray, &lpRect))
		{
			x = lpRect.right - nWidth;
			y = lpRect.top - nHeight;
		}
	}

	// Create the main window
	g_hwnd = CreateWindowEx(
		WS_EX_TOPMOST | WS_EX_TOOLWINDOW,			// extended style
		L"NS Ruby Notification",
		L"Ruby Notification",
		WS_POPUP,
		x,
		y,
		nWidth,
		nHeight,
		HWND_DESKTOP,
		NULL,
		hInstance,
		NULL
		);

	if(!g_hwnd) {
		return 0;
	}

	/* create the thread that will handle the communication with the ruby service */
	hCmdThread = CreateThread( (LPSECURITY_ATTRIBUTES)NULL, 0, srvcom, NULL, 0, NULL );
	if( hCmdThread == NULL )
	{
		Logger::instance()->log( GetLastError() );
		goto cleanup;
	}

	/* creates the global synchronization event */
	g_sync = CreateEvent( (LPSECURITY_ATTRIBUTES)NULL, TRUE, FALSE, L"rubysync" );
	
	/* creates the signaling event */
	g_signal = CreateEvent( (LPSECURITY_ATTRIBUTES)NULL, FALSE, FALSE, L"rubysignal" );

	show();

	/* start the message loop */
	while( (bRet = GetMessage( &msg, NULL, 0, 0 ))!= 0)
	{
		if(bRet == -1)
		{
			return 0;
		}
		else
		{
			TranslateMessage(&msg);
			DispatchMessage(&msg);
		}
	}

cleanup:
	cleanup();

	return (int)msg.wParam;
}

#pragma region Windowing

/* Changes history:
 *		2009-10-23 - nohros - Release
 */
LRESULT CALLBACK WndProc(HWND hwnd, UINT uMsg, WPARAM wParam, LPARAM lParam)
{
	switch(uMsg)
	{
		case WM_PAINT:
		case WM_ERASEBKGND:
			draw(hwnd, (HDC)wParam);
			return 1L;
		
		case UWM_MSG:
		default:
			return DefWindowProc(hwnd, uMsg, wParam, lParam);
	}
	return 0;
}

/* Changes history:
 *		2009-10-23 - nohros - Release
 */
void draw(HWND hwnd, HDC hdc)
{
	RECT rc;
	PAINTSTRUCT ps;
	HPEN hPen, hOldPen;
	HBRUSH hBrush, hOldBrush;

	// white brush and blue pen
	hBrush = (HBRUSH)GetStockObject(WHITE_BRUSH);
	hPen = CreatePen(PS_SOLID, 3, RGB(127, 157,185));

	GetClientRect(hwnd, &rc);

	BeginPaint(hwnd, &ps);
		hOldBrush = (HBRUSH)SelectObject(hdc, hBrush);
		hOldPen = (HPEN)SelectObject(hdc, hPen);
		Rectangle(hdc, rc.left, rc.top, rc.right, rc.bottom);
		SelectObject(hdc, hOldBrush);
		SelectObject(hdc, hOldPen);
		DeleteObject(hPen);
		DeleteObject(hBrush);
	EndPaint(hwnd, &ps);
}

/* Changes history:
 *		2009-10-23 - nohros - Release
 */
void show()
{
	char *pmsg;
	HANDLE hHeap;

	hHeap = GetProcessHeap();
	while(WaitForSingleObject(g_signal, INFINITE) == WAIT_OBJECT_0 && WaitForSingleObject(g_sync, 0) == WAIT_TIMEOUT) {

		// Show the window using the flag specified by the program
		// that started the application, and send the application a
		// WM_PAINT message.
		AnimateWindow(g_hwnd, 700, AW_SLIDE | AW_VER_NEGATIVE);

		while(!g_msgq.empty()) {
			pmsg = g_msgq.front();
			g_msgq.pop();
			HeapFree(hHeap, HEAP_ZERO_MEMORY, (LPVOID)pmsg);
		}
	}
}

/* Changes history:
 *		2009-10-23 - nohros - Release
 */
void hide(HWND hwnd)
{
	ShowWindow( hwnd, SW_HIDE );
}
#pragma endregion

DWORD WINAPI srvcom(LPVOID param)
{
	char msg[NOTIFY_BUFSIZE], *pmsg;
	DWORD dw;	
	HANDLE hPipe;

	HANDLE hHeap = GetProcessHeap();

	// gets the configuration instance
	Configuration *config = Configuration::instance();
	
CREATE_PIPE: /* creates the named pipe to handle the service communication */

	hPipe = CreateNamedPipe(L"\\\\.\\pipe\\rubygui",
		PIPE_ACCESS_DUPLEX,
		PIPE_TYPE_MESSAGE | PIPE_READMODE_MESSAGE | PIPE_WAIT,
		config->get_MaxInstances(),
		NOTIFY_BUFSIZE,
		NOTIFY_BUFSIZE,
		NMPWAIT_USE_DEFAULT_WAIT,
		NULL);

	if( hPipe == INVALID_HANDLE_VALUE )
	{
		Logger::instance()->log( GetLastError() );
		return 1;
	}

	// waits the client process(ruby service) to connect.
	do {
		if( !ConnectNamedPipe(hPipe, NULL) )
		{
			dw = GetLastError();
			if( dw != ERROR_PIPE_CONNECTED )
			{
				CloseHandle( hPipe );
				hPipe = INVALID_HANDLE_VALUE;
				Logger::instance()->log( dw );
				goto CREATE_PIPE;
			}
		}

		// read the client sent message
		if(!ReadFile( hPipe, msg, NOTIFY_BUFSIZE, &dw, NULL ))
		{
			CloseHandle( hPipe );
			hPipe = INVALID_HANDLE_VALUE;
			Logger::instance()->log( dw );
			goto CREATE_PIPE;
		}

		pmsg = (char*)HeapAlloc( hHeap, HEAP_ZERO_MEMORY, dw );
		if( pmsg == NULL ) {
			Logger::instance()->log( GetLastError() );
		}
		else {
			memcpy_s(pmsg, dw, msg, dw );
			g_msgq.push( pmsg ); /* enqueue the received msg */
			SetEvent( g_signal );
		}

		if(!DisconnectNamedPipe( hPipe ))
		{
			CloseHandle( hPipe );
			hPipe = INVALID_HANDLE_VALUE;
			Logger::instance()->log( dw );
			goto CREATE_PIPE;
		}
	} while( WaitForSingleObject(g_sync, 0) == WAIT_TIMEOUT );

	CloseHandle( hPipe );
	hPipe = INVALID_HANDLE_VALUE;

	return 0;
}

void cleanup()
{
	Logger::instance()->~Logger();
	Configuration::instance()->~Configuration();
}