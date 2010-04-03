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

#include "common/logging.h"

#if defined(OS_WIN)
#include <windows.h>
typedef HANDLE FileHandle;
typedef HANDLE MutexHandle;
#elif defined(OS_LINUX)
#include <sys/syscall.h>
#include <time.h>
#endif

#if defined(OS_POSIX)
#include <stdlib.h>
#include <stdio.h>
#include <string.h>
#include <unistd.h>
#define MAX_PATH PATH_MAX
typedef FILE* FileHandle;
typedef pthread_mutex_t* MutexHandle;
#endif

#include <ctime>
#include <iomanip>
#include <algorithm>

namespace logging {

const char* log_severity_names[LOG_NUM_SEVERITIES] = {
  "INFO", "WARNING", "ERROR", "ERROR_REPORT", "FATAL" };

int mim_log_level = 0;
LogLockingState lock_log_file = LOCK_LOG_FILE;

// Which log file to use? This is initialized by InitLogging or
// will be lazily initialized to the defaul value(On Windows, use afile
// next to the exe; on POSIX, where it may not even possible to locate the
// executable on disk, use stderr) when it is first need.

#if defined(OS_WIN)
typedef wchar_t PathChar;
typedef std::wstring PathString;
#else
typedef char PathChar;
typedef std::string PathString;
#endif
PathString* log_file_name = NULL;

// this file is lazily opened and the handle may be NULL
FileHandle log_file = NULL;

// what should be prepended to each message?
bool log_process_id = false;
bool log_thread_id = false;
bool log_timestamp = true;
bool log_tickcount = false;

// Using a global mutex for lock. We need to do this
// because LockFileEx is not thread safe.
#if defined(OS_WIN)
MutexHandle log_mutex = NULL;
#elif defined(OS_POSIX)
pthread_mutex_t log_mutex = PTHREAD_MUTEX_INITIALIZER;
#endif

// Helper functions to wrap plataform differences.

int32 CurrentProcessId() {
#if defined(OS_WIN)
  return GetCurrentProcessId();
#elif definde(OS_POSIX)
  return getpid()
#endif
}

int32 CurrentThreadId() {
#if defined(OS_WIN)
  return GetCurrentThreadId();
#elif defined(OS_LINUX)
  return syscall(__NR_gettid);
#endif
}

uint64 TickCount() {
#if defined(OS_WIN)
  return GetTickCount();
#elif defined(OS_LINUX)
  struct timespec ts;
  clock_gettime(CLOCK_MONOTONIC, &ts);

  uint64 absolute_micro = 
    static_cast<int64>(ts.tv_sec) * 1000000 +
    static_cast<int64>(ts.tv_nsec) / 1000;

  return absolute_micro;
#endif
}

void CloseFile(FileHandle) {
#if defined(OS_WIN)
  CloseHandle(log);;
#endif
  fclose(log);
#endif
}

// Called by logging functions to ensure that debug_file is initialized
// and can be used for writing. Returns false if the file could not be
// initialized. debug_file will be NULL in this case.
bool InitializeLogFileHandle() {
  if(log_file)
    return true;

  if(!log_file_name) {
    // Nobody has called InitLogging to specify a debug log file, so here we
    // intialize the log file name to a default.
#if defined(OS_WIN)
    // On windows we use the same path as exe.
    wchar_t module_name[MAX_PATH];
    GetModuleFileName(NULL, module_name, MAX_PATH);
    log_file_name = new std::wstring(module_name);
    std::wstring::size_type last_backslash = 
      log_file_name->rfind('\\', log_file_name->size());
    if(last_backslash != std::wstring::npos)
      log_file_name += L"debug.log";
#elif defined(OS_POSIX)
    // on other plataforms we just use the current directory.
    log_file_name = new std::string("debug.log");
#endif
  }

#if defined(OS_WIN)
  log_file = CreateFile(log_file_name->c_str(), GENERIC_WRITE,
                        FILE_SHARE_READ | FILE_SHARE_WRITE, NULL,
                        OPEN_ALWAYS, FILE_ATTRIBUTE_NORMAL, NULL);
  if(log_file == INVALID_HANDLE_VALUE || log_file == NULL) {
    // try the current directory
    log_file = CreateFile(L".\\debug.log", GENERIC_WRITE,
                          FILE_SHARE_READ | FILE_SHARE_WRITE, NULL,
                          OPEN_ALWAYS, FILE_ATTRIBUTE_NORMAL, NULL);
    if(log_file == INVALID_HANDLE_VALUE || log_file == NULL) {
      log_file = NULL;
      return false;
    }
  }
  SetFilePointer(log_file, 0, 0, FILE_END);
#elif
  log_file = fopen(log_file_name->c_str(), "a");
  if(log_file == NULL)
    return false;
#endif

  return true;
}

void InitLogMutex() {
#if defined(OS_WIN)
  if(!log_mutex) {
    // \ is not legal chracter in mutex names so we replace \ with /
    std::wstring safe_name(*log_file_name);
    std::replace(safe_name.begin(), safe_name.end(), '\\', '/');
    std::wstring t(L"Global\\");
    t.append(safe_name);
    log_mutex = ::CreateMutex(NULL, FALSE, t.c_str());
  }
#elif defined(OS_POSIX)
  // statically initialized
#endif
}

void InitLogging(const PathChar* new_log_file, LogLockingState lock_log) {
  // TODO==================================
}

} // namespace logging