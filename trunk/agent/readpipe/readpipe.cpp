#include "windows.h"
#include <iostream>

using namespace std;

void main(int argc, char* argv[])
{
	HANDLE hPipe;
	char buffer[4097];
	DWORD cbReaded, fResult;

	hPipe = CreateFile( L"\\\\.\\pipe\\notify", GENERIC_READ, 0, (LPSECURITY_ATTRIBUTES)NULL, OPEN_EXISTING, 0, NULL );
	if( hPipe == INVALID_HANDLE_VALUE )
	{
		cout << GetLastError();
		return;
	}

	while( fResult = ReadFile( hPipe, buffer, 4096, &cbReaded, NULL) )
		cout << buffer;

	CloseHandle( hPipe );
}