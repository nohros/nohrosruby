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

// For historical reasons, the windows.h header defaults to including the winsock.h
// header file for Windows Sockets 1.1. The WIN32_LEAN_AND_MEAN macro prevents the
// winsock.h from beign included by the windows.h header.
#define WIN32_LEAN_AND_MEAN
#define _WIN32_WINNT 0x0500

#ifndef AGENT_NET_AGENT_LISTENER_H__
#define AGENT_NET_AGENT_LISTENER_H__

#include "common/settings.h"

#include <windows.h>
#include <winsock2.h>
#include <ws2tcpip.h>
#include <stdio.h>
#include "common/constants.h"

#define LISTENER_STOPED		0x1
#define LISTENER_LISTENING	0x2

typedef struct notifybuf
{
	SOCKET fd; /* control connection */
	char inbuf[kDefaultBufferSize]; /* last received data */
	char *extra; /* extra characters */
	int extralen; /* number of extra chars */
	int len; /* number of chars last readed */
} notifybuf_t, *pnotifybuf_t;

typedef struct msgprocbuf
{
	SOCKET fd; /* control connection */
	char inbuf[kDefaultBufferSize]; /* last received data */
	int len; /* number of chars last readed */
	char *host; /* hostname:port */
} msgprocbuf_t, *pmsgprocbuf_t;

/* A structure used to transfer error messages or codes between
 * the message processor DLL and this class.
 *
 * If the "len" member of this structure is set to a value greater than zero,
 * the "E_un" member must be a pointer to a NULL terminated string and the "len"
 * must contain the number of characters(in TCHARS) within the string(whitout
 * the terminating NULL character); otherwise will be assumed that the "E_un" member
 * represents the code of the error returned by the GetLastError or WSALastError functions
 * or a code defined in the eventmessages.h header file.
 */
typedef struct dllerror_t {
	union {
		wchar_t* msg;
		unsigned long errorcode;
	} E_un;
	unsigned int len;
};

#define e_errorcode E_un.errorcode

#define e_msg E_un.msg

/* a structure that contains information about functions that can be used
 * by the msg processor DLL to predefined purposes( report error, send msg to server, ...).
 */
typedef struct RBCALLBACK {
	void (*dllerror)( dllerror_t* );
	int (*sendmsg)(char* msg, int length);
	bool (*read)(pmsgprocbuf_t msgbuf);
} *PRBCALLBACK;

typedef bool (*MSGSTART)(PRBCALLBACK);
typedef bool (*MSGPROC)(LPSTR, int);
typedef void (*MSGEND)();

typedef struct msgproc
{
	HMODULE module; /* the library handle */
	MSGSTART startup; /* pointer to the startup method */
	MSGPROC process; /* pointer to the process method */
	MSGEND end; /* pointer to the close method */
} msgproc_t, *pmsgproc_t;

class Listener
{
public:
	~Listener();

	// Singleton pattern
	static Listener *instance()
	{
		if(!s_listener)
			s_listener = new Listener();
		return s_listener;
	}

	/* Initializes the socket library and
	/* creates a thread to handle the server messages */
	bool startup();

private:
	Listener();

	static Listener *s_listener;

	/* handle to the command receiver thread */
	HANDLE m_CmdThread;

	/* synchronization event */
	HANDLE evtsync; 

	/* receiver buffer */
	notifybuf_t nbuf;

	/* the message processor library */
	msgproc_t msgproc;

	/* the callback structure */
	RBCALLBACK callback;

	bool m_isListening;

	/* main receiver function */
	static DWORD WINAPI recvmsg(LPVOID);

	/* Read data from the server
	 *
	 * The server will sent dat in packets. A message may consist
	 * of one or more packets. A packet always includes a packet
	 * header and is usually followed by packet data that contains
	 * the message. Each new message starts in a new packet.
	 *
	 * The packet header is always 8 bytes in length and states
	 * the Type and Length of the entire packet.
	 * 
	 *  ------------------------------------------------
	 * | Type	|	Status	|	Length	|	Reserved	|
	 * | 1 byte	|	1-byte	|	2-byte	|	4-byte		|
	 * | uchar  |	uchar	|	ushort	|	   -		|
	 *  ------------------------------------------------
	 */
	//static int read(notifybuf_t *notify);

	/* reads a block of data from the TCP buffer
	 * this function nmust be used to receive binary data from the server.
	 */
	static bool read(pmsgprocbuf_t msgbuf);

	/* reads a line from the current TCP buffer.
	 * @param notify pointer to a notifybuf_t structure.
	 */
	static int readline(notifybuf_t *notify);

	/* connects to the notify server 
	 * @param notify pointer to a notifybuf_t structure.
	 */
	static bool connect(notifybuf_t* notify);

	/* log an error generated by the message processor DLL.
	 * @param dllerror_t Pointer to a dllerror_t structure.
	 */
	static void dllerror( dllerror_t* );

	static int sendmsg( char *msg, int len );
};

#endif // AGENT_NET_AGENT_LISTENER_H__