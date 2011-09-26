# Copyright (c) 2011 Nohros Inc. All rights reserved.
# Use of this source code is governed by a MIT-style license that can be
# found in the LICENSE file.

# Call the .proto compiler for a specific language

import sys
import os
import subprocess

argv_len = len(sys.argv)
if argv_len < 2:
  print "Usage protos_compiler.py PROTO_COMPILER_FILE_PATH [proto files to compile] [proto dir]"
  sys.exit(0)
try:
  protoc_path = sys.argv[1]

  if argv_len < 4:
    proto_dir = sys.argv[2]
  else:
    # if the proto_dir was not specified the working directory will be used as the .proto container.
    proto_dir = os.getcwd()

  # What is specified, a proto directory or a list of proto files.
  if proto_dir.endswith(".proto"):
    # get all the arguments after the protoc path
    proto_files = sys.argv[2:]
  else:
    # find all *.protos files with filter
    proto_files = [proto_file for proto_file in os.listdir(proto_dir) if proto_file.endswith(".proto")]

  print "Nohros Inc. Protocol Buffer compiler invocation"
  print ""
  print "Compiling *.protos using: "
  print "  Compiler: " + protoc_path
  print "  Proto(s) path: " + proto_dir
  print "  Amount: " + str(len(proto_files))
  print ""
  
  # compile all the proto specified proto files
  for proto_file in proto_files:
    script = "\"" + protoc_path + "\" " + proto_dir+"\\"+proto_file
    print "Executing the compiler script"
    print "  " + script
    os.system(script)
    
except Exception as inst:
  print inst
  sys.exit(1)