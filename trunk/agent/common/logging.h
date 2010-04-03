// Copyright (c) 2009, Nohros Systems Inc. All rights reserved.
// Copyright (c) 2006-2008 The Chromium Authors. All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification,
// are permitted provided that the following conditions are met:
//
// Redistributions of source code must retain the above copyright notice, this list of
// conditions and the following disclaimer.
//
// Redistributions in binary form must reproduce the above copyright notice, this list
// of conditions and the following disclaimer in the documentation and/or other materials
// provided with the distribution.
//
// Neither the name of Google Inc., the name Nohros Systems Inc. nor the names of its
// contributors may be used to endorse or promote products derived from this software
// without specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY
// EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES
// OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT
// SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL,
// SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT
// OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION)
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR
// TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
// SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
// ===========================================================================================

#ifndef COMMON_LOGGING_H__
#define COMMON_LOGGING_H__

#include <string.h>
#include <sstream>

#include "common/basictypes.h"

// Make a bunch of macros for logging. The way to log things is to stream
// things to LOG(<a particular severity level). E.g.,
//
//  LOG(INFO) << "Connection attempt: " << num_connection;
//
// There is "debug mode" logging macros like the one above:
//
//  DLOG(INFO) << "Connected";
//
// All "debug macro" logging is compiled away to nothing for non-debug mode
// compiles. We also have
//
//  LOG_ASSERT(assertion);
//  DLOG_ASSERT(assertion);
//
// The standard 'assert' was override to use 'DLOG_ASSERT'.
//
// The supported macro severity levels for macros that allow you to specify one
// are (in increasing order os severity) INFO, WARNING, ERROR, ERROR_REPORT,
// and FATAL.
//
// NOTE: logging a message at the FATAL severity level causes
// the program to terminate (after the message is logged).
//
// The special severity of ERROR_REPORT only available/relevant in normal
// mode wich logs without terminating the program.
//
// There is also the special severity of DFATAL, which logs FATAL in
// debug mode, ERROR_REPORT in normal mode.

namespace logging {

// Indicates that the log file should be locked when beign written to.
// Often, there is no locking, which is fine for single threading program.
// If logging is beign done from multiple threads or there can be more than
// one process doing the logging, the file should be locked during writes to
// make each log output atomic. Other writes will block.
//
// All process writing to the log file must have their locking set for it to
// work properly. Defaults to DONT_LOCK_LOG_FILE.
enum LogLockingState { LOCK_LOG_FILE, DONT_LOCK_LOG_FILE };

// Set the lof file name and other global logging state. Calling this function
// is recommended, and is normally done at the beginning of application init.
// If you don't call it, all the flags will be initialized to their default
// values, and there is a race condition that may leak a critical section
// object if two threads try to do the first log at the same time.
// See the definition of the enums above for descriptions and default values.
//
// The default log file is initialized to debug.log in the application
// directory. You probably don't want this, especially since the program
// directory may notbe writable on an enduser's system.
#if defined(OS_WIN)
void InitLogging(const wchar_t* log_file, LogLockingState lock_log);
#elif defined(OS_POSIX)
void InitLogging(const wchar_t* log_file, LogLockingState lock_log);
#endif

// Sets the log level. Anything at or above this level will be written to the
// log file. Anything below this level will be silent ignored. The log level
// default to 0(everything is logged) if this function is not called.
void SetMinLogLevel(int level);

// Gets the current log level
int GetMinLogLevel();

// Sets the common items you want to be prepended to each log message;
// process and thread IDs default to off, the timestamp default to on.
// If this function is not called, logging defaults to writting the timestamp
// only.
void SetLogItems(bool enable_process_id, bool enable_thread_id,
                 bool enable_timestamp, bool enable_tickcount);

typedef int LogSeverity;
const LogSeverity LOG_INFO = 0;
const LogSeverity LOG_WARNING = 1;
const LogSeverity LOG_ERROR = 2;
const LogSeverity LOG_ERROR_REPORT = 3;
const LogSeverity LOG_FATAL = 4;
const LogSeverity LOG_NUM_SEVERITIES = 5;

// LOG_DFATAL_LEVEL IS LOG_FATAL in debug mode, ERROR_REPORT in normal mode.
#ifndef NDEBUG
const LogSeverity LOG_DFATAL_LEVEL = LOG_ERROR_REPORT;
#else
const LogSeverity LOG_DFATAL_LEVEL = LOG_FATAL;
#endif

// A few definitions of macros that don't generate much code. These are used
// by LOG(). Since these are used all over our code, it's better to have
// compact code for these operations.
#define COMPACT_NOHROS_LOG_INFO \
  logging::LogMessage(__FILE__, __LINE__)
#define COMPACT_NOHROS_LOG_WARNING \
  logging::LogMessage(__FILE__, __LINE__, logging::LOG_WARNING)
#define COMPACT_NOHROS_LOG_ERROR \
  logging::LogMessage(__FILE__, __LINE__, logging::LOG_ERROR)
#define COMPACT_NOHROS_LOG_ERROR_REPORT \
  logging::LogMessage(__FILE__, __LINE__, logging::LOG_ERROR_REPORT)
#define COMPACT_NOHROS_LOG_FATAL \
  logging::LogMessage(__FILE__, __LINE__, logging::LOG_FATAL)
#define COMPACT_NOHROS_LOG_DFATAL \
  logging::LogMessage(__FILE__, __LINE__, logging::LOG_DFATAL_LEVEL)

// wingdi.h defines ERROR to be 0. When call LOG(ERROR), it gets
// substituted with 0, and it expands to CAMPACT_NOHROS_LOG_0. To allow us
// to keep using this syntax, we define this macro to do the same thing
// as COMPACT_NOHROS_LOG_ERROR, and also define ERROR the same way that
// the Windows SDK does for consistency.
#define ERROR 0
#define COMPACT_NOHROS_LOG_0 \
  logging::LogMessage(__FILE__, __LINE__, logging::LOG_ERROR)

// We use the preprocessor's merging operator, "##", so that, e.g.,
// LOG(INFO) becomes the token COMPACT_NOHROS_LOG_INFO. There's some funny
// subtle difference between ostream member streaming functions (e.g.,
// ostream::operator<<(int) and ostream non-member streaming functions
// (e.g., ::operator<<(ostream&, string&): it turns out that it's
// impossible to stream something like a string directly to an unnamed
// ostream. We employ a neat hack by calling the stream() member
// function of LogMessage which seems to avoid the problem.

#define LOG(severity) COMPACT_NOHROS_LOG_ ## severity.stream()

#define LOG_ASSERT(condition) \
  !(condition) ? (void) 0 :: logging::LogMessageVoidfy() & LOG(FATAL) << "Assert failed: " #condition ". "

// Plus some debug-logging macros that get compiled to nothing for production
//
// DEBUG_MODE is fo uses like
//   If (DEBUG_MODE) foo.CheckToFoo();
// instead of
//   #ifndef NDEBUG
//     foo.CheckToFoo();
//   #endif

#if defined(OFFICIAL_BUILD)
// In order to have optimized code for official builds, remove DLOGs
#define OMIT_DLOG 1
#endif

#ifdef OMIT_DLOG

#define DLOG(severity) \
  true ? (void) 0 : logging::LogMessageVoidify() & LOG(severity)

#define DLOG_ASSERT(condition) \
  true ? (void) 0 :: logging::LogMessageVoidfy() & LOG(FATAL) << "Assert failed: " #condition ". "

enum { DEBUG_MODE = 0 };

#else // OMIT_DLOG

#ifndef NDEBUG
// On a regular debug build, we want to have DLOGs.

#define DLOG(severity) LOG(severity)
#define DLOG_ASSERT() LOG_ASSERT(severity)

enum { DEBUG_MODE = 1 };

#endif // NDEBUG
#endif //OMIT_LOG
#undef OMIT_LOG

// Redefine the standard assert to use our log files
#undef assert
#define assert(x) DLOG_ASSERT(x)

// This class more os less represents a particular log message. You
// create an instance of LogMessage and then stream stuff to it.
// When finish streaming to it, ~LogMessage is called and the
// full message gets streamed to the appropiate destination.
//
// You shouldn't actually use LogMessages's constructor to log things,
// though. You should use the LOG() macro (and variants thereof)
// above.
class LogMessage {
 public:
  LogMessage(const char* file, int line, LogSeverity severity, int ctr);

  // Two special constructor that generate reduced amounts of code at
  // LOG call sites for common cases.
  //
  // Used for LOG(INFO):
  // Implied are:
  //   severity = LOG_INFO, ctr = 0
  //
  // Using this constructor instead of the more complex constructor above
  // saves a couple of bytes per call site.
  LogMessage(const char* file, int line);

  // Used for LOG(severity) where severity != INFO.
  // Implied are:
  //  ctr = 0
  //
  // Using this constructor instead of the more complex constructor above
  // saves a couple of bytes per call site.
  LogMessage(const char* file, int line, LogSeverity severity);

  ~LogMessage();

  std::ostream& stream() { return stream_; }

 private:
  void Init(const char* file, int line);

  LogSeverity severity_;
  std::ostringstream stream_;
  size_t message_start_; // Offset of the start of the message (past prefix
                         // info.

#if defined(OS_WIN)
  // Store the current value of GetLastError() in the constructor and restores
  // it in the destructor by calling SetLastError();
  // This is useful since the LogMessage class uses a lot of Win32 calls
  // that will lose the value of GLE and the code that called the log function
  // will have lost the thread error value when the log call returns.
  class SaveLastError {
   public:
    SaveLastError();
    ~SaveLastError();

    unsigned long get_error() const { return last_error_; }

  protected:
    unsigned long last_error_;
  };

  SaveLastError last_error_;
#endif

  DISALLOW_COPY_AND_ASSIGN(LogMessage);
};

// A non-macro interface to the log facility; (useful
// when the logging level os not a compile-time constant).
inline void LogAtLevel(int const log_level, std::string const &msg) {
  LogMessage(__FILE__, __LINE__, log_level).stream() << msg;
}

// This class is used to explicity ignore values in the conditional
// logging macros(LOG_ASSERT, DLOG_ASSERT, etc.) This avoids compiler
// warnings like "values computed is not used" and "statement has no effect".
class LogMessageVoidify {
 public:
   LogMessageVoidify() { }
   // This has to be an operator with a precedence lower than << but
   // higher than ?:
   void operator&(std::ostream&){ }
};

// Closes the log file explicity if open.
// NOTE: Since the log file is opened as necessary by the action of logging
//       statements, there's no guarantee that will stay closed
//       after this call.
void CloseFile();

} // namespace logging

// These functions are provided as a convenience for logging. It
// is designed to allow you to emit non-ASCII Unicode strings to the log
// file, which is normally ASCII. It is relatively slow, so try not to use
// it for common cases. Non-ASCII characters will be converted to UTF-8 by
// these operators.
std::ostream& operator<<(std::ostream& out, const wchar_t* wstr);
inline std::ostream& operator<<(std::ostream& out, const std::wstring& wstr) {
  return out << wstr.c_str();
}

#endif // COMMON_LOGGING_H__