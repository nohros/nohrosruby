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

#include "config.h"
#include "logging.h"

// Allocating and initializing Configuration's
// static data member. The pointer is beign
// allocated - not the object inself.
Configuration *Configuration::s_config = 0;

/* Changes history:
 *		2009-10-23 - nohros - Release
 */
Configuration::Configuration()
{
	szKeyName = TEXT("SOFTWARE\\Nohros\\Notify\\");
	hWatcherEvent = NULL;

	m_language = NULL;
	m_szAddr = NULL;
	m_timeout = NULL;
	m_msgproc = NULL;
	m_maxInstances = NULL;

	// load the configuration data
	loadAndWatch();

	// ensures taht all variables has a value
	loadDefaultValues();
}

/* Changes history:
 *		2009-10-23 - nohros - Release
 */
void Configuration::loadDefaultValues()
{
	if( m_language == NULL )
		m_language = MAKELANGID(LANG_NEUTRAL, SUBLANG_NEUTRAL);

	if( m_szAddr == NULL )
		m_szAddr = L"192.168.203.97:8955";

	if( m_timeout == NULL )
		m_timeout = 120000;

	if( m_msgproc == NULL )
		m_msgproc = L"msgproc.dll";

	if( m_maxInstances == NULL || m_maxInstances >  PIPE_UNLIMITED_INSTANCES)
		m_maxInstances = 10;
}

/* Changes history:
 *		2009-10-23 - nohros - Release
 */
void Configuration::load(HKEY hKey)
{
	LONG lRet;
	DWORD pvData;
	DWORD pcbData = sizeof(DWORD);
	LPWSTR lpszData = NULL;

	lpszData = (LPWSTR)HeapAlloc( GetProcessHeap(), HEAP_ZERO_MEMORY, MAX_BUFFER_SIZE*sizeof(WCHAR) );

	// get the language
	lRet = RegQueryValueEx( hKey, L"Language", NULL, NULL, (LPBYTE)&pvData, &pcbData );
	if(lRet == ERROR_SUCCESS)
		m_language = *(&pvData);

	//get the default timeout
	lRet = RegQueryValueEx( hKey, L"Timeout", NULL, NULL, (LPBYTE)&pvData, &pcbData );
	if( lRet == ERROR_SUCCESS )
		m_timeout = *(&pvData);

	/* get the maximum number of pipe instances that can be created. */
	lRet = RegQueryValueEx( hKey, L"MaxInstances", NULL, NULL, (LPBYTE)&pvData, &pcbData );
	if( lRet == ERROR_SUCCESS )
		m_maxInstances = *(&pvData);

	// get the server name
	regQueryString( hKey,  L"ServerName", &lpszData, &m_szAddr );

	// get the message processor dll name
	regQueryString( hKey, L"MsgProcModule", &lpszData, &m_msgproc );

	// get the event message DLL.
	regQueryString( hKey, L"EventMessage", &lpszData, &m_msgevent );

	if(lpszData != NULL)
		HeapFree( GetProcessHeap(), HEAP_ZERO_MEMORY, (LPVOID)lpszData);
}

/* Changes history:
 *		2009-12-17 - nohros - Release
 */
void Configuration::regQueryString(HKEY hKey, LPCWSTR lpValueName, LPWSTR *lpszData, LPWSTR *lpszValue)
{
	DWORD pcbData;
	LONG lRet;

	// get the server name
	lRet = RegQueryValueEx( hKey, lpValueName, NULL, NULL, NULL, &pcbData );
	if( lRet == ERROR_SUCCESS )
	{
		if( *lpszData == NULL || pcbData > MAX_BUFFER_SIZE )
		{
			LPVOID nw = HeapReAlloc( GetProcessHeap(), HEAP_ZERO_MEMORY | HEAP_REALLOC_IN_PLACE_ONLY, (LPVOID)*lpszData, pcbData );
			if( nw != NULL )
				*lpszData = (LPWSTR)nw;
		}

		lRet = RegQueryValueEx( hKey, lpValueName, NULL, NULL, (LPBYTE)*lpszData, &pcbData);
		if( lRet == ERROR_SUCCESS )
		{
			*lpszValue = (LPWSTR)HeapAlloc( GetProcessHeap(), HEAP_ZERO_MEMORY, pcbData );
			if( lpszValue != NULL )
			{
				pcbData = pcbData/sizeof(WCHAR);
				wcscpy_s( *lpszValue, pcbData, *lpszData );
			}
		}
	}
}

/* Changes history:
 *		2009-10-23 - nohros - Release
 */
void Configuration::watch()
{
	HKEY hKey;
	DWORD lRet;

	lRet = RegOpenKeyEx( HKEY_LOCAL_MACHINE, szKeyName, 0, KEY_NOTIFY | KEY_QUERY_VALUE, &hKey );
	if( lRet != ERROR_SUCCESS )
		return;

	if ( hWatcherEvent == NULL )
	{
		hWatcherEvent = CreateEvent( NULL, TRUE, FALSE, TEXT("WatcherThread") );
		if( hWatcherEvent == NULL )
		{
			Logger::instance()->log( GetLastError() );
			return;
		}
	}

	// reload the configuration value if the
	// attributes or contents of a ..\\Notify
	// registry key changes.
	while( true )
	{
		RegNotifyChangeKeyValue(hKey, TRUE, REG_NOTIFY_CHANGE_LAST_SET, hWatcherEvent, TRUE);

		if( this->STATUS == CONFIG_STOP_PENDING )
		{
			break;
		}

		if( WaitForSingleObject(hWatcherEvent, INFINITE) == WAIT_FAILED )
			break;

		load(hKey);

		ResetEvent( hWatcherEvent );
	}

	RegCloseKey(hKey);

	return;
}

/* Changes history:
 *		2009-10-23 - nohros - Release
 */
void Configuration::loadAndWatch()
{
	HKEY hKey = NULL;
	DWORD lRet;

	lRet = RegOpenKeyEx( HKEY_LOCAL_MACHINE, szKeyName, 0, KEY_READ, &hKey );
	if( lRet != ERROR_SUCCESS )
		return;

	this->load( hKey );

	// create a new thread to monitor the registry
	CreateThread(
		NULL,
		0,
		_threadstart,
		this,
		0,
		NULL);
}

/* Changes history:
 *		2009-10-23 - nohros - Release
 */
DWORD WINAPI Configuration::_threadstart(LPVOID param)
{
	((Configuration*)param)->watch();
	return 0;
}

/* Changes history:
 *		2009-10-23 - nohros - Release
 */
DWORD Configuration::get_Language()
{
	return this->m_language;
}

/* Changes history:
 *		2009-10-23 - nohros - Release
 */
LPWSTR Configuration::get_ServerName()
{
	return this->m_szAddr;
}

/* Changes history:
 *		2009-10-23 - nohros - Release
 */
DWORD Configuration::get_DefaultTimeout()
{
	return this->m_timeout;
}

LPWSTR Configuration::get_MsgModule()
{
	return this->m_msgproc;
}

LPWSTR Configuration::get_EventMsgModule()
{
	return this->m_msgevent;
}

DWORD Configuration::get_MaxInstances()
{
	return this->m_maxInstances;
}

/* Changes history:
 *		2009-10-23 - nohros - Release
 */
Configuration::~Configuration()
{
	this->STATUS = CONFIG_STOP_PENDING;

	// release the registry monitor
	SetEvent( hWatcherEvent );

	WaitForSingleObject( hWatcherEvent, 30000 );

	CloseHandle( hWatcherEvent );

	s_config = NULL;
}