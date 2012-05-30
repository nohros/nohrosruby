This directory contains the *.proto definitions and the python
build scripts used to convert the *.proto into language specific
classes.

*****************************************************
ProtoBuf classes build common steps:
 * Install Python


*****************************************************
ProtoBuf classes build steps for csharp:

 * Downlod the protogen from http://code.google.com/p/protobuf-csharp-port/
 * Install Python
 * Run local_csharp_proto_parser_generator.py

The generated class files will be located at folder parsers/csharp/


*****************************************************

ProtoBuf classes build steps for c:

 * Download protoc from http://code.google.com/p/protobuf/downloads/list
 * Run local_c_proto_parser_generator.py

The generated class files will be located at folder parsers/c/