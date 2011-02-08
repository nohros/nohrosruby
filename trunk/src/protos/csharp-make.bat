:: Copyright (c) 2010 Nohros Systems Inc.
:: 
:: Permission is hereby granted, free of charge, to any person obtaining a copy of this 
:: software and associated documentation files (the "Software"), to deal in the Software 
:: without restriction, including without limitation the rights to use, copy, modify, merge, 
:: publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons 
:: to whom the Software is furnished to do so, subject to the following conditions:
:: 	
:: The above copyright notice and this permission notice shall be included in all copies or 
:: substantial portions of the Software.
::
:: THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, 
:: INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR 
:: PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE 
:: FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR 
:: OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
:: DEALINGS IN THE SOFTWARE.
::
:: Author: neylor.silva@nohros.com(Neylor Ohmaly)
::
:: Generate CSharp classes for all the .proto files that resides into the running directory.
::
@echo off

set BIN_PATH=..\third_party\protobuf-csharp-port
set PROTOC=%BIN_PATH%\protoc.exe
set PROTOGEN=%BIN_PATH%\protogen.exe
set OUT_PATH=.\parsers\csharp

%PROTOC% --descriptor_set_out=%1bin --include_imports ruby_message_packet.proto
%PROTOGEN% %1bin

:: clean up residual files
if exist DescriptorProtoFile.cs del /q DescriptorProtoFile.cs
if exist CSharpOptions.cs del /q CSharpOptions.cs
del /y *.protobin

:: move the generated classes to the output folder
move /y *.cs %OUT_PATH%