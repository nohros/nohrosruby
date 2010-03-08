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

#include "logging.h"
#include "config.h"

// Allocating and initializing Logger's
// static data member. The pointer is beign
// allocated - not the object inself.
Logger *Logger::s_logger = 0;

Logger::Logger()
{
	DWORD nSize, nOffset;
	WCHAR szModulePath[MAX_PATH];
	WCHAR *pch;
	SYSTEMTIME st;

	// Get the path of the log file
	ZeroMemory( szModulePath, sizeof(szModulePath) );
	nSize = GetModuleFileName( NULL, szModulePath, MAX_PATH );
	if( nSize > 0 )
	{
		pch=wcsrchr( szModulePath, L'\\' );
		nOffset = (pch - szModulePath + 17); /* +16 means "\ ... */
											/* ... file name mask: nn_aaaammdd.txt, 15 chars +1[ null terminator ] */
		lpszBasePath = (LPWSTR)HeapAlloc( GetProcessHeap(), HEAP_ZERO_MEMORY, sizeof(WCHAR)*nOffset );
		if( lpszBasePath != NULL )
		{
			if(wcsncpy_s( lpszBasePath, nOffset, szModulePath, nOffset-16 )== 0)
			{
				GetSystemTime(&st);
				wsprintf((LPWSTR)&lpszBasePath[nOffset-16], L"nn_%d%02d%02d.log\0", st.wYear, st.wMonth, st.wDay);
			}
			else
			{
				ZeroMemory( lpszBasePath, sizeof(WCHAR)*nOffset ); /* The current directory will be used. */
			}
		}
	}
}

void Logger::log(DWORD dwEventId, ...)
{
	WORD wFacility = 0, wSeverity = 0;
	LPWSTR lpszFacility, lpszSeverity;

	// Get the event severity and facility
	wSeverity = LOWORD(dwEventId >> 30);
	wFacility = LOWORD( (dwEventId >> 16) & 0xFFF );

	switch( wSeverity )
	{
		case STATUS_SEVERITY_SUCCESS:
			lpszSeverity = L"SUCCESS";
			break;

		case STATUS_SEVERITY_INFORMATIONAL:
			lpszSeverity = L"INFO";
			break;

		case STATUS_SEVERITY_WARNING:
			lpszSeverity = L"WARN";
			break;

		case STATUS_SEVERITY_ERROR:
			lpszSeverity = L"ERROR";
			break;

		default:
			lpszSeverity = L"UNKNOWN";
			break;
	}

	switch( wFacility )
	{
		case WINSOCK_CATEGORY:
			lpszFacility = L"WINSOCK";
			break;

		case SERVICE_CATEGORY:
			lpszFacility = L"NTSERVICE";
			break;

		case UI_CATEGORY:
			lpszFacility = L"UI";
			break;

		case RUNTIME_CATEGORY:
			lpszFacility = L"Runtime";
			break;

		default:
			lpszFacility = L"GENERIC";
			break;
	}

	va_list args;
	va_start(args, dwEventId);
		log( dwEventId, lpszSeverity, lpszFacility, &args );
	va_end(args);
}

void Logger::log(DWORD dwEventId, LPWSTR lpszSeverity, LPWSTR lpszFacility, va_list *args)
{
	LPWSTR lpszBuffer = NULL;
	DWORD nSize = 0, cSize = 0, flags = 0, dwPos = 0, dwBytesWritten = 0;
	HMODULE hMsgLib = NULL; // handle to the message-only dll
	HANDLE hFile, hHeap;
	DWORD dwLangId;
	BOOL bOverflow;
	PDWORD_PTR pdwArgs = NULL;
	SYSTEMTIME st;

	dwLangId = Configuration::instance()->get_Language();
	hHeap = GetProcessHeap();

	hFile = CreateFile(lpszBasePath,
				GENERIC_WRITE,
				FILE_SHARE_READ,
				NULL,
				OPEN_ALWAYS,
				0,
				NULL);

	GetSystemTime(&st);

	// If the log file cound not be opened or created,
	// the log operation cannot be performed.
	if( hFile == INVALID_HANDLE_VALUE )
		goto cleanup;

	// If the bit 29 of the dwEventId is not set then the message
	// is a system message
	if( ((dwEventId >> 29) & 1) == 1 )
	{
		hMsgLib = LoadLibrary( Configuration::instance()->get_EventMsgModule() );

		// get the application error string
		// or the system error string if the application
		// message file could not be loaded
		if( hMsgLib == NULL )
		{
			dwEventId = GetLastError();
			flags |= FORMAT_MESSAGE_FROM_SYSTEM;
		}
		else
		{
			flags |= FORMAT_MESSAGE_FROM_HMODULE;
		}
	}
	else
	{
		flags |= FORMAT_MESSAGE_FROM_SYSTEM;
	}

	// Allocate the buffer that will hold the formated message
	cSize = sizeof(WCHAR)*(MAX_BUFFER_SIZE);
	lpszBuffer = (LPWSTR)HeapAlloc( hHeap, HEAP_ZERO_MEMORY, cSize );
	if( lpszBuffer == NULL )
		goto cleanup;

	// Message format : SEVERITY FACILITY HH:MM:SS MESSAGE\r\n
	nSize = wcslen( lpszSeverity ) + wcslen( lpszFacility ) + 12; /* +12 : 3[\t]; [ HH:MM:SS ](8); 2[\r\n]; 1[ null terminator ] */

	if( nSize < MAX_BUFFER_SIZE )
	{
		nSize = wsprintf( lpszBuffer, L"%s\t%s\t%02d:%02d:%02d\t", lpszSeverity, lpszFacility, st.wHour, st.wMinute, st.wSecond );
	}
	else
	{
		nSize = wsprintf( lpszBuffer, L"%s\t%s\t%d02:%d02:%d02\t", L"OVERFLOW", L"OVERFLOW", st.wHour, st.wMinute, st.wSecond );
	}

	// The application error
	cSize = FormatMessage(
		flags,
		(LPCVOID)hMsgLib,
		dwEventId,
		dwLangId,
		&lpszBuffer[nSize],
		MAX_BUFFER_SIZE-nSize,
		args
		);

	/* ...== MAX_BUFFER_SIZE - no space left for the system message; +1 [null terminator] */
	bOverflow = ((cSize+nSize+1) == MAX_BUFFER_SIZE);
	if( cSize == 0 || bOverflow )
	{
		// The message library is loaded only if the message
		// id belongs to the application message ID. So, we
		// need to load it here if they are not loaded yet
		if( hMsgLib == NULL )
			hMsgLib = LoadLibrary(L"NNEventMessages.dll");

		if( hMsgLib != NULL )
		{
			if( bOverflow || ((dwEventId = GetLastError()) == ERROR_INSUFFICIENT_BUFFER) )
			{
				dwEventId = NNMSG_RUNTIME_MESSAGE_TOO_LONG;
			}
			else if( dwEventId == ERROR_MR_MID_NOT_FOUND )
			{
				dwEventId = NNMSG_RUNTIME_ID_NOT_FOUND;
			}

			pdwArgs = (PDWORD_PTR)HeapAlloc( hHeap, HEAP_ZERO_MEMORY, sizeof(DWORD_PTR) );
			if( pdwArgs != NULL )
				pdwArgs[0] = dwEventId;

			cSize = FormatMessage(
				FORMAT_MESSAGE_FROM_HMODULE | FORMAT_MESSAGE_ARGUMENT_ARRAY,
				(LPCVOID)hMsgLib,
				dwEventId,
				dwLangId,
				&lpszBuffer[nSize],
				MAX_BUFFER_SIZE-nSize,
				(va_list*)pdwArgs
				);
			dwEventId = GetLastError();

			if( pdwArgs != NULL )
				HeapFree( hHeap, NULL, (LPVOID)pdwArgs );
		}
		else
		{
			// At this point the message library could not be loaded and
			// the text that describles the cause of that is too long.
			cSize = wcsncpy_s( &lpszBuffer[nSize], MAX_BUFFER_SIZE-nSize, L"The message library could not be loaded.\r\n", 42 );
		}
	}

	nSize += cSize;
	cSize = nSize*sizeof(WCHAR);

	dwPos = SetFilePointer( hFile, 0, NULL, FILE_END );

	LockFile( hFile, dwPos, 0, cSize, 0);
	WriteFile( hFile, lpszBuffer, cSize, &dwBytesWritten, NULL );
	UnlockFile( hFile, dwPos, 0, cSize, 0);

cleanup:
	// Release resources
	if( hMsgLib != NULL )
		::FreeLibrary( hMsgLib );

	if( lpszBuffer != NULL )
		HeapFree( hHeap, 0, lpszBuffer );

	CloseHandle(hFile);
}

Logger::~Logger()
{
	if( lpszBasePath != NULL )
		HeapFree( GetProcessHeap(), 0, lpszBasePath );
	s_logger = NULL;
}