# Copyright (c) 2011 Nohros Inc. All rights reserved.
# Use of this source code is governed by a MIT-style license that can be
# found in the LICENSE file.

# Call the .proto compiler for a the csharp language using the protobuf-csharp-port
# as the proto compiler. This script was created to be called by double clicking it
# on windows machines.

import os

os.system('proto_parser_generator.py "protogen.exe --descriptor_set_out=./parsers/csharp/ruby_message_packet.protobin -ignore_google_protobuf=true -namespace=Nohros.Ruby --include_imports -output_directory=./parsers/csharp/" .')
os.system('pause');