#pragma region MIT License
/*
 * Nohros Notify - Default message processor
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

#include "msgproc.h"

BOOL WINAPI DllMain(HINSTANCE hInst, DWORD fdwReason, LPVOID lpvReserveds)
{
	switch(fdwReason)
	{
		case DLL_PROCESS_ATTACH:
			DisableThreadLibraryCalls(hInst);
			break;

		case DLL_THREAD_ATTACH:
		case DLL_THREAD_DETACH:
		case DLL_PROCESS_DETACH:
			break;
	}
	return TRUE;
}

bool startup( PRBCALLBACK p )
{
	callback = p;
	procheap = GetProcessHeap();

	/* stack set-up */
	stack.head = (PRBNODE)HeapAlloc( procheap, HEAP_ZERO_MEMORY, sizeof(RBNODE) );
	if( stack.head == NULL )
	{
		error_t.e_errorcode = GetLastError();
		error_t.len = 0;
		callback->dllerror( &error_t );

		return false;
	}

	stack.sp = (PRBNODE)HeapAlloc( procheap, HEAP_ZERO_MEMORY, sizeof(RBNODE) );
	if( stack.sp == NULL )
	{
		error_t.e_errorcode = GetLastError();
		error_t.len = 0;
		callback->dllerror( &error_t );

		return false;
	}

	stack.head->data = NULL;
	stack.head->next = NULL;
	stack.head->size = 0;
	stack.sp = stack.head;

	return true;
}

bool process(char *msg, int length)
{
	char c, *ps, *p;
	int len = length-1;
	PRBCOMMAND pc;
	bool ret = false;
	
	/* to upper */
	for(int i = 0; i < length; i++)
	{
		c = *(msg+i);
		if(c >= 'a' && c <= 'z') {
			*(msg+i) = c-32;
		}
	}

	/* invalid command */
	c = msg[0];
	if( c < 'A' || c > 'Z' )
		return false;

	/* parse command	
	 *  The command is the first string before the first occurrence of the
	 *  space character in the string pointed by msg, or, when the command has
	 *  no parameter, the entire string pointed by msg.
	 */
	ps=msg;
	p=strchr( ps, ' ');
	if( p == NULL ) {
		p = &msg[len];
	}
	else
	{
		stack.head->data = ps;
		stack.head->next = NULL;
		stack.head->pi = 'C';
		stack.head->size = p-ps;

		/* parse parameters
		 *  A parameter is composed by a parameter identification and a parameter value.
		 *  Parameter identification is composed of a  starting space, a slash(/),
		 *  a single character and a ending space. Parameter value is a string without any parameter
		 *  identification in it.
		 */
		while(p < &msg[len])
		{
			ps=p;
			p=strchr(ps, '/');

			if(p == NULL)
				p=&msg[len];

			if( (*(p-1) == *(p+2)) && (*(p-1) == ' '))
			{
				ps=p+3;
				p=strchr(ps, '/');
				
				// the last parameter?
				if( p == NULL )
					p=&msg[len]+1;

				if( !new_node(&stack.sp->next) )
					return free_stack();

				// fix stack
				stack.length++;
				stack.sp->data[stack.sp->size] = '\0';
				stack.sp = stack.sp->next;

				// set up node values
				stack.sp->pi = *(ps-2);
				stack.sp->data = ps;
				stack.sp->size = p-ps;
				stack.sp->next = NULL;
			}
		}
	}

	/* handle the command */
	pc = commands[c-'A'].cmds;
	do {
		if( strcmp(pc->cmd, stack.head->data) ==0)
		{
			ret = pc->proc( );
			break;
		}
	}while( strcmp(pc++->cmd, "NOP") !=0);

	/* deallocate stack */
	free_stack();

	return ret;
}

void end()
{
	free_stack();

	HeapFree(procheap, HEAP_ZERO_MEMORY, stack.head );
}

bool free_stack()
{
	int i=0;
	PRBNODE node;
	PRBNODE *nodes;
	
	if(stack.length == 0)
		return true;

	nodes = (PRBNODE*)HeapAlloc( procheap, HEAP_ZERO_MEMORY, stack.length );

	/* stack walkthrough */
	node = stack.head->next;
	while( node != NULL )
	{
		nodes[i++] = node;
		node = node->next;
	}

	/* free the allocated resources */
	while(i-- > 0) {
		nodes[i]->size = 0;
		nodes[i]->next = NULL;
		HeapFree( procheap, HEAP_ZERO_MEMORY, (LPVOID)nodes[i] );
	}

	/* reset the head node */
	stack.head->size = 0;
	stack.head->next = NULL;
	stack.sp = stack.head;
	
	stack.length = 0;

	return true;
}

bool new_node(PRBNODE* node)
{
	*node = (PRBNODE)HeapAlloc( procheap, HEAP_ZERO_MEMORY, sizeof(RBNODE) );
	if( *node == NULL )
	{
		error_t.e_errorcode = GetLastError();
		error_t.len = 0;
		callback->dllerror( &error_t );

		return false;
	}
	return true;
}

#pragma region commands

bool callproc()
{
	STARTUPINFO si;
	PROCESS_INFORMATION pi;
	int nsize, len;
	char szPath[MAX_PATH], *pch, *name;
	RBNODE* pnode;

	/* must have only one nodes */
	pnode = stack.head->next;
	if( pnode == NULL || pnode->pi != 'N' || pnode->data == NULL )
		return false;

	// get the application directory path
	nsize = GetModuleFileNameA( NULL, szPath, MAX_PATH );
	if ( nsize == 0 ) {
		error_t.e_errorcode = GetLastError();
		error_t.len = 0;
		callback->dllerror( &error_t );

		return false;
	}

	// build the file path
	pch = strrchr( szPath, L'\\' )+1; // remove the file name from the path
	if( (pch-szPath)+pnode->size > MAX_PATH ) {
		error_t.e_errorcode = NNMSG_RUNTIME_MAX_PATH;
		error_t.len = 0;
		callback->dllerror( &error_t );

		return false;
	}

	memcpy( pch+7, name, pnode->size ); // +7(plugin folder+1) file name
	memcpy( pch, "plugin\\", 7 ); // plugin folder
	len += pch-szPath+7;

	ZeroMemory( &si, sizeof(si) );
	si.cb = sizeof(si);
	ZeroMemory( &pi, sizeof(pi) );

	// start the process
	if ( !CreateProcessA(
		szPath,	// module name
		NULL,	// process handle not inheritable
		NULL,	// process handle not inheritable
		NULL,	// thread handle not inheritable
		FALSE,	// no inheritence
		0,		// no creation flags
		NULL,	// parent environment
		NULL,	// parent starting directory
		(LPSTARTUPINFOA)&si,
		&pi)
		) {
			error_t.e_errorcode = GetLastError();
			error_t.len = 0;
			callback->dllerror( &error_t );

			return false;
	}

	CloseHandle( pi.hProcess );
	CloseHandle( pi.hThread );

	return true;
}

bool msi()
{
	return true;
}

bool nop()
{
	return true;
}

bool update()
{
	int size = 0, len;
	char *name = NULL;
	bool ret = true;

	PRBNODE pnode;
	msgprocbuf_t msgbuf;

	pnode = stack.head->next;
	while( pnode != NULL )
	{
		switch(pnode->pi)
		{
			case 'S': // the DLL size in bytes
				size = strtol(pnode->data, NULL, 10);
				break;

			case 'N': // the DLL name
				len = pnode->size;
				name = (char*)HeapAlloc( procheap, HEAP_ZERO_MEMORY, (len*sizeof(char))+1 );
				if( name == NULL ) {
					error_t.e_errorcode = GetLastError();
					error_t.len = 0;
					callback->dllerror( &error_t );

					ret = false;
					break;
				}
				memcpy( name, pnode->data, len*sizeof(char) );
				name[len] = '\0';
				break;

			case 'H': // the host name used to download the data
				len = pnode->size;
				msgbuf.host = (char*)HeapAlloc( procheap, HEAP_ZERO_MEMORY, (len*sizeof(char))+1 );
				if( msgbuf.host == NULL ) {
					error_t.e_errorcode = GetLastError();
					error_t.len = 0;
					callback->dllerror( &error_t );

					ret = false;
					break;
				}
				memcpy( msgbuf.host, pnode->data, len*sizeof(char) );
				msgbuf.host[len] = '\0';
		}
		pnode = pnode->next;
	}

	// reset message buffer
	ZeroMemory( msgbuf.inbuf, NOTIFY_BUFSIZE );
	msgbuf.fd = INVALID_SOCKET;
	msgbuf.len = 0;

	// download the DLL
	if( size >0 && name != NULL && msgbuf.host != NULL )
	{
		ret = download( name, len, &msgbuf );
		if( !ret )
		{
			error_t.e_errorcode = NNMSG_RUNTIME_DATA_DOWNLOAD;
			error_t.len = 0;
			callback->dllerror( &error_t );
		}
		else
		{
			// TODO: Update the runing DLL.
		}
	}

	if( name != NULL )
		HeapFree( procheap, HEAP_ZERO_MEMORY, (LPVOID)name );

	if( msgbuf.host != NULL )
		HeapFree( procheap, HEAP_ZERO_MEMORY, (LPVOID)msgbuf.host );

	return ret;
}

bool show()
{
	DWORD cbWritten;
	HANDLE hPipe;
	PRBNODE pnode;

	/* must have only one nodes */
	pnode = stack.head->next;
	if( pnode == NULL || pnode->pi != 'M' || pnode->data == NULL )
		return false;

	/* send the message to the ruby application to be displayed */
	hPipe = CreateFile(
		L"\\\\.\\pipe\\rubygui",
		GENERIC_WRITE,
		0,
		(LPSECURITY_ATTRIBUTES)NULL,
		OPEN_EXISTING,
		0,
		NULL);

	if(hPipe == INVALID_HANDLE_VALUE)
	{
		error_t.e_errorcode = GetLastError();
		error_t.len = 0;
		callback->dllerror( &error_t );
		return false;
	}

	if(!WriteFile( hPipe, pnode->data, pnode->size, &cbWritten, NULL))
	{
		error_t.e_errorcode = GetLastError();
		error_t.len = 0;
		callback->dllerror( &error_t );
		
		CloseHandle(hPipe);
		return false;
	}

	return true;
}

#pragma endregion

bool download( char *name, int len, pmsgprocbuf_t msgbuf )
{
	int nsize;
	char szPath[MAX_PATH];
	char* pch;
	bool ret;

	HANDLE fp;
	DWORD cbBytesWritten;

	ZeroMemory( szPath, MAX_PATH );

	// get the application directory path
	nsize = GetModuleFileNameA( NULL, szPath, MAX_PATH );
	if ( nsize == 0 ) {
		error_t.e_errorcode = GetLastError();
		error_t.len = 0;
		callback->dllerror( &error_t );

		return false;
	}

	// build the file path
	pch = strrchr( szPath, L'\\' )+1; // remove the file name from the path
	if( (pch-szPath)+len > MAX_PATH ) {
		error_t.e_errorcode = NNMSG_RUNTIME_MAX_PATH;
		error_t.len = 0;
		callback->dllerror( &error_t );

		return false;
	}

	memcpy( pch+7, name, len ); // +8(update folder+1) file name
	memcpy( pch, "update\\", 7 ); // update folder
	len += pch-szPath+7;

	// download and save the file
	fp = CreateFileA(
		szPath,
		GENERIC_WRITE,
		FILE_SHARE_READ,
		NULL,
		CREATE_ALWAYS,
		0,
		NULL
		);

	if( fp == INVALID_HANDLE_VALUE ) {
		error_t.e_errorcode = GetLastError();
		error_t.len = 0;
		callback->dllerror( &error_t );

		return false;
	}

	callback->read( msgbuf );
	while( msgbuf->len > 0 )
	{
		ret = WriteFile( fp, msgbuf->inbuf, msgbuf->len, &cbBytesWritten, (LPOVERLAPPED)NULL );
		if (!ret ) {
			error_t.e_errorcode = GetLastError();
			error_t.len = 0;
			callback->dllerror( &error_t );
			break;
		}
		callback->read( msgbuf );
	}

	// ensure that the connection is closed
	if( msgbuf->fd != INVALID_SOCKET )
		while( msgbuf->len > 0 ) callback->read( msgbuf );


	CloseHandle( fp );

	return ret;
}