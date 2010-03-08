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

#ifndef _NOTIFY_MSGPROC_
#define _NOTIFY_MSGPROC_

#include "..\notify\eventmessages.h"
#include "..\notify\listener.h"
#include "rbstack.h"
#include "windows.h"

/**/
PRBCALLBACK callback;

/* the process heap handle */
HANDLE procheap;

/* error callback structure */
dllerror_t error_t;

#ifdef __cplusplus
extern "C"
{
#endif

/* starts the library */
__declspec(dllexport) bool startup(PRBCALLBACK);

/* process the message */
__declspec(dllexport) bool process(char* msg, int length);

/* ends the library */
__declspec(dllexport) void end();

#ifdef __cplusplus
}
#endif

/* a structure which contains information on the commands this program can understand */
typedef bool (*RBCMDHANDLER)();
typedef struct RBCCOMMAND
{
	char *cmd;				/* command name */
	RBCMDHANDLER proc;		/* function to call to do the job */
} *PRBCOMMAND;

/* a structure which contains the commands that this program understand */
typedef struct RBVOCABULARY {
	PRBCOMMAND cmds; /* pointer to an array of commands */
}*PRBVOCABULARY;

/* the command stack */
RBSTACK stack;

/* create a new RBNODE data structure */
bool new_node( PRBNODE *node );

/* release resource used by the ruby command stack */
bool free_stack();

#pragma region commands handlers

/* nop command handler */
bool nop();

/* updates this DLL version */
bool update();

/* download and instal a MSI cabinet */
bool msi();

/* displays a message to the logged-on user */
bool show();

/* creates a process */
bool callproc();

/* download binary files from the server
 * @param name The name of the file.
 * @param len Length (in characters) of the name.
 * @param size The size(in bytes) of the file.
 */
bool download( char *name, int len, pmsgprocbuf_t msgbuf );

#pragma endregion

#pragma region commands arrays

// NOP command vocabulary
RBCCOMMAND rb_cmd[] = {
	{ "NOP", nop }
};

// CALLPROC command vocabulary
RBCCOMMAND rb_cmd_c[] = {
	{ "CALLPROC", callproc }
};

// M command vocabulary
RBCCOMMAND rb_cmd_m[] = {
	{ "MSI", msi },
	{ "NOP", nop }
};

// S command vocabulary
RBCCOMMAND rb_cmd_s[] = {
	{ "SHOW", show },
};

// U command vocabulary
RBCCOMMAND rb_cmd_u[] = {
	{ "UPDATE", update },
	{ "NOP", nop }
};

#pragma endregion

/* indexed commands array
 * each slot represents a ASCII code(shifted 41 positions) of a characters of the english alphabet */
RBVOCABULARY commands[26] = {
	rb_cmd, /* A */
	rb_cmd, /* B */
	rb_cmd_c, /* C */
	rb_cmd, /* D */
	rb_cmd, /* E */
	rb_cmd, /* F */
	rb_cmd, /* G */
	rb_cmd, /* H */
	rb_cmd, /* I */
	rb_cmd, /* J */
	rb_cmd, /* K */
	rb_cmd, /* L */
	rb_cmd_m, /* M */
	rb_cmd, /* N */
	rb_cmd, /* O */
	rb_cmd, /* P */
	rb_cmd, /* Q */
	rb_cmd, /* R */
	rb_cmd_s, /* S */
	rb_cmd, /* T */
	rb_cmd_u, /* U */
	rb_cmd, /* V */
	rb_cmd, /* W */
	rb_cmd, /* X */
	rb_cmd, /* Y */
	rb_cmd  /* Z */
};

#endif // _NOTIFY_MSGPROC_