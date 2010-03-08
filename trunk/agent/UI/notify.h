#include <windows.h>
#include <iostream>
#include <queue>

#ifndef _NOTIFY_UI_
#define _NOTIFY_UI_

#define ID_TIMER	10*1000
#define	UWM_MSG		(WM_USER + 100)

// Window instance
HINSTANCE hinst;

/* main window handle */
HWND g_hwnd;

/* message processor thread */
HANDLE hCmdThread;
HANDLE g_sync, g_signal;

/* message queue */
std::queue<char*> g_msgq;

LRESULT CALLBACK WndProc(HWND hwnd, UINT uMsg, WPARAM wParam, LPARAM lParam);
int WINAPI WinMain(HINSTANCE, HINSTANCE, LPSTR, int);

/* handle the communication with the ruby service
 * @param param NULL
 */
DWORD WINAPI srvcom(LPVOID param);

void show();
void hide( HWND hwnd );

/* performs the final clean up */
void cleanup();

void draw(HWND hwnd, HDC hdc);
void displayMsg(HWND hwnd, LPCWSTR lpString);

#endif // _NOTIFY_UI_