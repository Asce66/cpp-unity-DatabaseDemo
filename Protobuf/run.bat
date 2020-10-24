protoc_server.exe --cpp_out=../Server/DatabaseDemoServer MsgUser.proto
protoc_server.exe --cpp_out=../Server/DatabaseDemoServer MsgData.proto

protoc_client.exe --csharp_out=../Client/Assets/Scripts/Proto/Protobuf MsgUser.proto
protoc_server.exe --csharp_out=../Client/Assets/Scripts/Proto/Protobuf MsgData.proto

pause