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

#ifndef _REGISTRY_
#define _REGISTRY_

#define MAX_BUFFER_SIZE		1024
#define NOTIFY_BUFSIZE		4096

// Config status
#define CONFIG_RUNNING		0x00000001
#define CONFIG_STOP_PENDING	0x00000002

#include "windows.h"

class Configuration
{
public:
	~Configuration();

	// Singleton pattern
	static Configuration *instance()
	{
		if(!s_config)
			s_config = new Configuration();
		return s_config;
	}

	/* Gets the default language */
	DWORD get_Language();

	/* Gets the address of the ruby server */
	LPWSTR get_ServerName();

	/* Gets the default timeout */
	DWORD get_DefaultTimeout();

	/* Gets the name of the DLL that will handle the commands. */
	LPWSTR get_MsgModule();

	/* Gets the name of the resource-only DLL. */
	LPWSTR get_EventMsgModule();

	/* Gets the maximum number of instances of the rubygui application
	 * that can be running at the same time
	 */
	DWORD get_MaxInstances();

private:
	Configuration();

	static Configuration *s_config;
	static DWORD WINAPI _threadstart(LPVOID param);

	DWORD m_language;
	LPWSTR m_szAddr; // the server address
	DWORD m_sPort; // the server port
	DWORD m_timeout; // default timeout
	LPWSTR m_msgproc; // address of the message processor DLL.
	LPWSTR m_msgevent; // address of the event message DLL.
	DWORD m_maxInstances; // maximuum number of GUI instances that can be created

	LPTSTR szKeyName;
	HANDLE hWatcherEvent;
	DWORD STATUS;

	/*
	 * Watch for modification in the windows registry.
	 */
	void watch();

	/*
	 * Load the configuration files and reload then if the
	 * registry is modified.
	 */
	void loadAndWatch();

	/*
	 * Read the windows registry and load the configuration values.
	 *		hKey: Handle to an open key
	 */
	void load(HKEY hKey);

	/*
	 * Ensure that configuration values are valid before the values can be used.
	 */
	void loadDefaultValues();

	/*
	 * Gets a string value from the registry
	 */
	void regQueryString(HKEY, LPCWSTR, LPWSTR*, LPWSTR*);
};
#endif