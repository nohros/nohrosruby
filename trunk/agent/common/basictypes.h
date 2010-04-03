// Copyright (c) 2009, Nohros Systems Inc.
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

#ifndef COMMMON_BASICTYPES_H__
#define COMMMON_BASICTYPES_H__

#include "common/port.h"    // Types that only need exist on certain systems

typedef signed char       schar;
typedef signed char       int8;
typedef short             int16;
typedef int               int32;

// NOTE: unsigned types are DANGEROUS in loops and other arithmetical
// places. Use the signed types unless your variable represents a bit
// pattern (eg a hash value) or you really need the extra bit. Do NOT
// use 'unsigned' to express "this value alwas be positive";
// use assertion for this.

typedef unsigned char   uint8;
typedef unsigned short  uint16;
typedef unsigned int    uint32;

// A type to represent a Unicode code-point value. As of Unicode 4.0,
// such values require up to 21 bits.
// ( For type-checking on pointers, make this explicity signed,
// and it should alwas be the signed version of whatever int32 is.)
typedef signed int    char32;

const uint8   kuint8max   = (( uint8) 0xff);
const uint16  kuint16max  = ((uint16) 0xffff);
const uint32  kuint32max  = ((uint32) 0xffffffff);
const int8    kint8min    = ((  int8) 0x80);
const int8    kint8max    = ((  int8) 0x7f);
const int16   kint16min   = (( int16) 0x8000);
const int16   kint16max   = (( int16) 0x7fff);
const int32   kint32min   = (( int32) 0x80000000);
const int32   kint32max   = (( int32) 0x7FFFFFFF);

// A macro to disallow the copy constructor and operator= functions
// This should be used in the private: declaration for a class.
#define DISALLOW_COPY_AND_ASSIGN(TypeName) \
  TypeName(const TypeName&);               \
  void operator=(const TypeName&)

// A macro ro disallow all the implicity constructors, namely the
// default constructor, copy constructor and operator= functions.
//
// This should be used in private: declarations for a class
// that wants to prevent anyone from instantiating it. This is
// especially useful for classes containing only static methods.
#define DISALLOW_IMPLICIT_CONSTRUCTORS(TypeName) \
  TypeName();                                    \
  DISALLOW_COPY_AND_ASSIGN(TypeName)

#endif // COMMMON_BASICTYPES_H__