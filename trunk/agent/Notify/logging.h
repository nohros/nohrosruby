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

#ifndef _LOGGING_
#define _LOGGING_

#define MAX_LOGMESSGAE_LEN	1024

#include "windows.h"
#include "eventmessages.h" 

// Event logger class
class Logger
{
public:
	~Logger();

	// Singleton pattern
	static Logger *instance()
	{
		if(!s_logger)
			s_logger = new Logger();
		return s_logger;
	}

	// Logs an event
	void log(DWORD dwEventId, ...);

private:
	static Logger *s_logger;

	// The path of he log files
	LPWCH lpszBasePath;

	// Performs the log operation
	void log(DWORD dwEventId, LPWSTR lpszSeverity, LPWSTR lpszFacility, va_list *args);

	// private constructor
	Logger();
};

#ifdef _DEBUG
	static Logger *m_logger = Logger::instance();
	#define TRACE(x)	m_logger->log( NNMSG_GENERIC_INFORMATIONAL , x);
#else
	#define TRACE(x)
#endif

#endif /* _LOGGING_ */