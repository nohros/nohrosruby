/*
 eventmessages.mc

 Copyright (c) 2008-2009 by Nohros Systems Inc <www.nohros.com>
 This file is part of the Notify application.

*/
 //Message definitions
 // Service Category
//
//  Values are 32 bit values layed out as follows:
//
//   3 3 2 2 2 2 2 2 2 2 2 2 1 1 1 1 1 1 1 1 1 1
//   1 0 9 8 7 6 5 4 3 2 1 0 9 8 7 6 5 4 3 2 1 0 9 8 7 6 5 4 3 2 1 0
//  +---+-+-+-----------------------+-------------------------------+
//  |Sev|C|R|     Facility          |               Code            |
//  +---+-+-+-----------------------+-------------------------------+
//
//  where
//
//      Sev - is the severity code
//
//          00 - Success
//          01 - Informational
//          10 - Warning
//          11 - Error
//
//      C - is the Customer code flag
//
//      R - is a reserved bit
//
//      Facility - is the facility code
//
//      Code - is the facility's status code
//
//
// Define the facility codes
//
#define WINSOCK_CATEGORY                 0x100
#define UI_CATEGORY                      0x102
#define RUNTIME_CATEGORY                 0x104
#define SERVICE_CATEGORY                 0x101
#define GENERIC_CATEGORY                 0x103


//
// Define the severity codes
//
#define STATUS_SEVERITY_WARNING          0x2
#define STATUS_SEVERITY_SUCCESS          0x0
#define STATUS_SEVERITY_INFORMATIONAL    0x1
#define STATUS_SEVERITY_ERROR            0x3


//
// MessageId: NNMSG_SERVICE_CREATEEVENT
//
// MessageText:
//
//  Cannot create the global syncronization event.
//
#define NNMSG_SERVICE_CREATEEVENT        ((DWORD)0xE1010100L)

//
// MessageId: NNMSG_SERVICE_CTRLDISPATCHER
//
// MessageText:
//
//  Cannot starts the service control dispatcher.
//
#define NNMSG_SERVICE_CTRLDISPATCHER     ((DWORD)0xE1010101L)

 // Generic messages
//
// MessageId: NNMSG_GENERIC_SUCCESS
//
// MessageText:
//
//  Debug: %1.
//
#define NNMSG_GENERIC_SUCCESS            ((DWORD)0x21030200L)

//
// MessageId: NNMSG_GENERIC_INFORMATIONAL
//
// MessageText:
//
//  Debug: %1.
//
#define NNMSG_GENERIC_INFORMATIONAL      ((DWORD)0x61030201L)

//
// MessageId: NNMSG_GENERIC_WARNING
//
// MessageText:
//
//  %1.
//
#define NNMSG_GENERIC_WARNING            ((DWORD)0xA1030201L)

//
// MessageId: NNMSG_GENERIC_ERROR
//
// MessageText:
//
//  %1.
//
#define NNMSG_GENERIC_ERROR              ((DWORD)0xE1030202L)

 // Runtime category
//
// MessageId: NNMSG_RUNTIME_MESSAGE_TOO_LONG
//
// MessageText:
//
//  Length of error message is longer than the maximum allowed(1024 characters).Id:%1.
//
#define NNMSG_RUNTIME_MESSAGE_TOO_LONG   ((DWORD)0xE1040300L)

//
// MessageId: NNMSG_RUNTIME_ID_NOT_FOUND
//
// MessageText:
//
//  The system cannot find message text for message number %1!lu!.
//
#define NNMSG_RUNTIME_ID_NOT_FOUND       ((DWORD)0xE1040301L)

//
// MessageId: NNMSG_RUNTIME_NO_MEMORY
//
// MessageText:
//
//  There is not enough memory %1.
//
#define NNMSG_RUNTIME_NO_MEMORY          ((DWORD)0xE1040302L)

//
// MessageId: NNMSG_RUNTIME_DATA_DOWNLOAD
//
// MessageText:
//
//  I/O error occured while download data from the RUBY server.
//
#define NNMSG_RUNTIME_DATA_DOWNLOAD      ((DWORD)0xE1040303L)

//
// MessageId: NNMSG_RUNTIME_MAX_PATH
//
// MessageText:
//
//  Path too long.
//
#define NNMSG_RUNTIME_MAX_PATH           ((DWORD)0xE1040304L)

//
// MessageId: NNMSG_RUNTIME_INVALMESSAGE
//
// MessageText:
//
//  The message is not NULL terminated or number of character exceed the maximum allowed.
//
#define NNMSG_RUNTIME_INVALMESSAGE       ((DWORD)0xE1040305L)

 // Winsock category
//
// MessageId: NNMSG_WINSOCK_CONNECTION_ATTEMPT
//
// MessageText:
//
//  The connection has been lost. Attempting to reconnect to server. Connection attempt: %1.
//
#define NNMSG_WINSOCK_CONNECTION_ATTEMPT ((DWORD)0x61000400L)

