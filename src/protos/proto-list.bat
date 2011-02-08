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
:: List all .proto files that resides into the running directory and call the specified language
:: specific batch compiler(ex. csharp-make, java-make).
::
@echo off

:: check if the language specific batch compiler name was specified and exists.
if [%1]==[] goto COMPILER_FLAG
if not exist %1 goto COMPILER_FLAG

for /f %%a in ('dir /b *.proto') do %1 %%a
pause

:COMPILER_FLAG
echo. Language specific compiler batch name not passed or
echo. does not exists. The compiler must resides in the same
echo. directory that this batch is running.