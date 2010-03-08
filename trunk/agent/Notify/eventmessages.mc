;/*
; eventmessages.mc
;
; Copyright (c) 2008-2009 by Nohros Systems Inc <www.nohros.com>
; This file is part of the Notify application.
;
;*/

SeverityNames=(Success=0x0:STATUS_SEVERITY_SUCCESS
			  Informational=0x1:STATUS_SEVERITY_INFORMATIONAL
			  Warning=0x2:STATUS_SEVERITY_WARNING
			  Error=0x3:STATUS_SEVERITY_ERROR)

FacilityNames=(Winsock=0x100:WINSOCK_CATEGORY
			   NTService=0x101:SERVICE_CATEGORY
			   UI=0x102:UI_CATEGORY
			   Generic=0x103:GENERIC_CATEGORY
			   Runtime=0x104:RUNTIME_CATEGORY)

LanguageNames=(English=0x0409:MSG00409)
LanguageNames=(Portuguese=0x0416:MSG00416)

; //Message definitions

; // Service Category

MessageIdTypeDef=DWORD

MessageId=0x100
Severity=Error
Facility=NTService
SymbolicName=NNMSG_SERVICE_CREATEEVENT
Language=English
Cannot create the global syncronization event.
.

Language=Portuguese
O evento de sincronização global não pode ser criado.
.

MessageId=0x101
Severity=Error
Facility=NTService
SymbolicName=NNMSG_SERVICE_CTRLDISPATCHER
Language=English
Cannot starts the service control dispatcher.
.

Language=Portuguese
O thread de controle não pode ser iniciado.
.

; // Generic messages
MessageId=0x200
Severity=Success
Facility=Generic
SymbolicName=NNMSG_GENERIC_SUCCESS
Language=English
Debug: %1.
.

Language=Portuguese
Debug: %1.
.

MessageId=0x201
Severity=Informational
Facility=Generic
SymbolicName=NNMSG_GENERIC_INFORMATIONAL
Language=English
Debug: %1.
.

Language=Portuguese
Debug: %1.
.

MessageId=0x201
Severity=Warning
Facility=Generic
SymbolicName=NNMSG_GENERIC_WARNING
Language=English
%1.
.

Language=Portuguese
%1.
.

MessageId=0x202
Severity=Error
Facility=Generic
SymbolicName=NNMSG_GENERIC_ERROR
Language=English
%1.
.

Language=Portuguese
%1.
.

; // Runtime category
MessageId=0x300
Severity=Error
Facility=Runtime
SymbolicName=NNMSG_RUNTIME_MESSAGE_TOO_LONG
Language=English
Length of error message is longer than the maximum allowed(1024 characters).Id:%1.
.

Language=Portuguese
O Comprimento da mensagem de erro é superior ao máximo permitido(1024 caracteres).Id:%1.
.

MessageId=0x301
Severity=Error
Facility=Runtime
SymbolicName=NNMSG_RUNTIME_ID_NOT_FOUND
Language=English
The system cannot find message text for message number %1!lu!.
.

Language=Portuguese
O sistema não pode encontrar o texto para para mensagem de número %1!lu!.
.

MessageId=0x302
Severity=Error
Facility=Runtime
SymbolicName=NNMSG_RUNTIME_NO_MEMORY
Language=English
There is not enough memory %1.
.

Language=Portuguese
Não há memoria suficiente %1.
.

MessageId=0x303
Severity=Error
Facility=Runtime
SymbolicName=NNMSG_RUNTIME_DATA_DOWNLOAD
Language=English
I/O error occured while download data from the RUBY server.
.

Language=Portuguese
Um erro de I/O ocorreu ao realizar o download dos dados do servidor RUBY.
.

MessageId=0x304
Severity=Error
Facility=Runtime
SymbolicName=NNMSG_RUNTIME_MAX_PATH
Language=English
Path too long.
.

Language=Portuguese
O nome do caminho e muito longo.
.

MessageId=0x305
Severity=Error
Facility=Runtime
SymbolicName=NNMSG_RUNTIME_INVALMESSAGE
Language=English
The message is not NULL terminated or number of character exceed the maximum allowed.
.

Language=Portuguese
A menssagem nao possui o caracter NULL como finalizador or o numero de caracteres excede o maximo permitido.
.

; // Winsock category
MessageId=0x400
Severity=Informational
Facility=Winsock
SymbolicName=NNMSG_WINSOCK_CONNECTION_ATTEMPT
Language=English
The connection has been lost. Attempting to reconnect to server. Connection attempt: %1.
.

Language=Portuguese
A conexao foi perdida. Tentando reconectar-se ao servidor. Tentativa de conexao: %1!lu!.
.
