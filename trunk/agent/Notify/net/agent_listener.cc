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

#include "agent/net/agent_listener.h"

#include "common/settings.h"
#include "third_party/glog/logging.h"

// Allocating and initializing Configuration's
// static data member. The pointer is beign
// allocated - not the object inself.
Listener *Listener::s_listener = 0;

Listener::Listener()
{
	// Global initialization code	
	WORD wVersionRequired;
	WSADATA wsaData;
	wVersionRequired = MAKEWORD(2, 2);	

	m_isListening = false;

	if ( WSAStartup(wVersionRequired, &wsaData) != 0)
		return;

	nbuf.fd = INVALID_SOCKET;
	nbuf.len = 0;
	nbuf.extralen = 0;
	nbuf.extra = NULL;
	*nbuf.inbuf = NULL;

	/* alloc the notify buffer structure */
	//nbuf = (notifybuf_t*)HeapAlloc( GetProcessHeap(), HEAP_ZERO_MEMORY, sizeof(*nbuf) );
	//if ( nbuf == NULL )
		//return;
}

bool Listener::startup()
{
	if( m_isListening )
	{
		TRACE(L"Listener is already running.")
		return true;
	}

	/* create the notification event */
	this->evtsync = CreateEvent( NULL, TRUE, FALSE, L"Nohros listener sync" );
	if( this->evtsync == NULL )
	{
		Logger::instance()->log( GetLastError() );
		TRACE(L"Failed to create the nohros listener synchronization event.")
	}

	msgproc.module = LoadLibrary( Configuration::instance()->get_MsgModule() );
	if( msgproc.module == NULL )
	{
		Logger::instance()->log( GetLastError() );
		TRACE(L"The message processor DLL could not ne loaded.")
		return false;
	}

	/* check if the msgproc library is valid */
	msgproc.startup = reinterpret_cast<MSGSTART>(GetProcAddress(msgproc.module, "startup"));
	msgproc.process = reinterpret_cast<MSGPROC>(GetProcAddress(msgproc.module, "process"));
	msgproc.end = reinterpret_cast<MSGEND>(GetProcAddress(msgproc.module, "end"));
	if( msgproc.startup == NULL || msgproc.process == NULL || msgproc.end == NULL )
	{
		Logger::instance()->log( GetLastError() );
		TRACE(L"The message processor DLL does not implemets all the required methods")
		return false;
	}

	// create the callback structure
	callback.read = read;
	callback.sendmsg = sendmsg;
	callback.dllerror = dllerror;

	TRACE(L"Creating the message reader thread")

	// create the message reader thread
	m_CmdThread = CreateThread( NULL, 0, recvmsg, (LPVOID)this, 0, NULL );
	if ( m_CmdThread == NULL )
	{
		Logger::instance()->log( GetLastError() );
		closesocket( nbuf.fd );
		return false;
	}

	return m_isListening = true;
}

bool Listener::connect(notifybuf_t* notifybuf)
{
	sockaddr_in service;
	int nSize = sizeof(service);

	// close the previous socket
	if( notifybuf->fd != INVALID_SOCKET )
	{
		shutdown( notifybuf->fd, SD_BOTH );
		closesocket( notifybuf->fd );
	}	

	TRACE( L"Creating a new socket" )

	SOCKET sSocket = socket( AF_INET, SOCK_STREAM, IPPROTO_TCP );
	if( sSocket == INVALID_SOCKET )
	{
		Logger::instance()->log( WSAGetLastError() );
		return false;
	}

	TRACE( L"Connecting to the socket server" )

	service.sin_family = AF_INET;
	if( WSAStringToAddress( Configuration::instance()->get_ServerName(), AF_INET, NULL, (LPSOCKADDR)&service, &nSize ) == SOCKET_ERROR )
	{
		Logger::instance()->log( WSAGetLastError() );
		return false;
	}
	//service.sin_port = htons( Configuration::instance()->get_ServerPort() );

	if( ::connect( sSocket, (SOCKADDR*)&service, sizeof(service) ) == SOCKET_ERROR )
	{
		Logger::instance()->log( WSAGetLastError() );
		return false;
	}

	notifybuf->fd = sSocket;
	return true;
}

DWORD WINAPI Listener::recvmsg(LPVOID param)
{
	char szMsg[NOTIFY_BUFSIZE+1];
	int i = 0;
	
	Configuration *config = Configuration::instance();
	Logger *logger = Logger::instance();
	Listener *listener = instance();

	ZeroMemory( szMsg, NOTIFY_BUFSIZE );

	TRACE(L"Entering in message loop")

	do {
		if( connect( &listener->nbuf ) )
		{
			// starts the message processor DLL up.
			listener->msgproc.startup( &listener->callback );
			
			// message loop
			i=0;
			while( readline(&listener->nbuf) > 0 )
			{
				// reads a line from the server and send it to message processor.
				listener->msgproc.process( listener->nbuf.inbuf, listener->nbuf.len );
			}
		}
		logger->log( NNMSG_WINSOCK_CONNECTION_ATTEMPT, i++ );
	} while( WaitForSingleObject(listener->evtsync, config->get_DefaultTimeout()) == WAIT_TIMEOUT );

	listener->msgproc.end();

	TRACE(L"The recvmsg thread has ended")

	return 0;
}

bool Listener::read(pmsgprocbuf_t msgbuf)
{
	sockaddr_in service;
	int size;
	char *pch;
	
	// create a new socket if no one exists
	if( msgbuf->fd == INVALID_SOCKET )
	{
		TRACE( L"Creating a socket to transfer the binary data" )
		
		size = sizeof(service);

		msgbuf->fd = socket( AF_INET, SOCK_STREAM, IPPROTO_TCP );
		if( msgbuf->fd == INVALID_SOCKET )
		{
			Logger::instance()->log( WSAGetLastError() );
			return false;
		}

		pch = strchr( msgbuf->host, ':' );
		if( pch == NULL )
		{
INVALID_ADDR:
			TRACE( L"Invalid host address" );
			Logger::instance()->log( NNMSG_GENERIC_ERROR, L"Invalid host address" );
			return false;
		}
		*pch='\0';

		service.sin_family = AF_INET;
		service.sin_port = htons( atoi(pch+1) );
		service.sin_addr.s_addr = inet_addr( msgbuf->host );

		if( service.sin_addr.s_addr == INADDR_NONE )
			goto INVALID_ADDR;


		TRACE( L"Connecting to the socket server" )
		if( ::connect( msgbuf->fd, (SOCKADDR*)&service, sizeof(service) ) == SOCKET_ERROR )
		{
			Logger::instance()->log( WSAGetLastError() );
			return false;
		}
	}

	// read the data from the server
	if( (msgbuf->len = recv(msgbuf->fd, msgbuf->inbuf, NOTIFY_BUFSIZE, 0)) < 1)
	{
		if( msgbuf->len == SOCKET_ERROR)
		{
			Logger::instance()->log( WSAGetLastError() );
			TRACE( L"Error while retrieving binary data from the server" )

			shutdown( msgbuf->fd, SD_BOTH );
			closesocket( msgbuf->fd );
			msgbuf->fd = INVALID_SOCKET;

			return false;
		}
	}

	// closing the socket
	if( msgbuf->len == 0 || msgbuf->len == WSAECONNRESET )
	{
		shutdown( msgbuf->fd, SD_BOTH );
		closesocket( msgbuf->fd );
		
		msgbuf->fd = INVALID_SOCKET;
	}
	return true;
}

int Listener::readline(notifybuf_t *notify)
{
	int size, rcvd;
	char *data, *eol;

	/* shift the extra to the front */
	size = NOTIFY_BUFSIZE;
	rcvd = 0;
	if( notify->extra )
	{
		memmove( notify->inbuf, notify->extra, notify->extralen );
		rcvd = notify->extralen;
	}

	data = notify->inbuf;
	notify->len = 0;

	// receive loop
	do {
		size -=rcvd;

		// loop ultil a EOL is reached
		for(eol = data; rcvd; rcvd--, eol++)
		{
			notify->len++;
			if(*eol == '\r')
			{
				*eol = '\0'; /* ensure NULL terminated */
				notify->extra = eol + 1;

				// CRLF?
				if(rcvd > 1 && *(eol + 1) == '\n')
				{
					notify->extra++;
					rcvd--;
				}

				// have more data?
				if((notify->extralen = --rcvd) == 0)
				{
					notify->extra = NULL;
				}
				return 1;
			}
			else if(*eol == '\n')
			{
				*eol = '\0'; /* ensure NULL terminated */
				notify->extra = eol + 1;

				// have more data?
				if((notify->extralen = --rcvd) == 0)
				{
					notify->extra = NULL;
				}
				return 1;
			}
		}

		data = eol;
		if( (rcvd = recv(notify->fd, data, size, 0)) < 1)
		{			
			// the chars received until now will be discarded.
			notify->len = 0;
			return rcvd;
		}
	}while (size);

	return 0;
}

void Listener::dllerror( dllerror_t* error_t)
{
	TRACE( L"Error from message processor")

	Logger *logger = Logger::instance();

	if( error_t->len == 0 )
		logger->log( error_t->e_errorcode );
	else
	{
		// NULL terminated?
		if( error_t->e_msg[error_t->len] != L'\0' || error_t->len > MAX_LOGMESSGAE_LEN )
			logger->log( NNMSG_RUNTIME_INVALMESSAGE );
		else
			logger->log( NNMSG_GENERIC_ERROR, error_t->e_msg );
	}
}

int Listener::sendmsg(char *msg, int len)
{
	int sent = send( instance()->nbuf.fd , msg, len, 0);
	if( sent == SOCKET_ERROR )
		Logger::instance()->log( WSAGetLastError() );
	return sent;
}

Listener::~Listener()
{
	DWORD lRet;

	SetEvent( evtsync );

	if( nbuf.fd != INVALID_SOCKET )
		shutdown( nbuf.fd, SD_BOTH );

	lRet = WaitForSingleObject( m_CmdThread, INFINITE );

	if( nbuf.fd != INVALID_SOCKET )
		closesocket( nbuf.fd );

	m_isListening = false;

	// Global destruction
	WSACleanup();
}