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
:: Generate Java classes for all the .proto files that resides into the running directory.
::
@echo off

:: Creating a newline variable. Note that the two blank lines are required.
set NLM=^


set newline=^^^%NLM%%NLM%^%NLM%%NLM%
set BIN_PATH=..\third_party\google\protobuf
set PROTOC=%BIN_PATH%\protoc.exe
set OUT_PATH=.\parsers\java

echo    %1 %newline%
%PROTOC% --java_out=%OUT_PATH% %1

:: clean up residual files
REM if exist DescriptorProtoFile.cs del /q DescriptorProtoFile.cs
REM if exist CSharpOptions.cs del /q CSharpOptions.cs
REM del /q *.protobin

:: move the generated classes to the output folder
REM move /y *.cs %OUT_PATH%