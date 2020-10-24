#pragma once
#include<memory>
#include"MsgUser.pb.h"

enum ProtoID
{
	C2SLogin = 0,
	S2CLogin,
	C2SRegister,
	S2CRegister,
	C2SWriter,
	S2CWriter,
	C2SAddWriters,
	S2CAddWriters,
	C2SBook,
	S2CBook,
	C2SAddBooks,
	C2SDeleteBooks,
	UpdateBook,
	C2SUpdateBooks,
	C2SYanTao,
	S2CYanTao,
	C2SWeiWen,
	S2CWeiWen,
	C2SWriteBook,
	S2CWriteBook,
	C2SDoYanTao,
	S2CDoYanTao,
	C2SDoWeiWen,
	S2cDoWeiWen,
	C2SAddWriteBook,
	C2SAddDoWeiWen,
	C2SAddDoYanTao,
	C2SDeleteWriters,
	C2SUpdateWriters,
	C2SAddYanTao,
	UpdateYanTao,
	C2SUpdateYanTao,
	C2SDeleteYanTao,
	C2SDepartment,
	S2CDepartment,
	C2SAddDepartment,
	UpdateDepartment,
	C2SUpdateDepartment,
	C2SDeleteDepartment,
	C2SAddWeiWen,
	UpdateWeiWen,
	C2SUpdateWeiWen,
	C2SDeleteWeiWen,
	C2SSearchDoYanTao,
	S2CSearchDoYanTao,
	UpdateDoYanTao,
	C2SUpdateDoYanTao,
	C2SDeleteDoYanTao,
	C2SSearchWriteBook,
	S2CSearchWriteBook,
	C2SSearchDoWeiwen,
	S2CSearchDoWeiwen
};

class ProtobufParser
{
public:
	template<class T>
	static std::shared_ptr<T>Decode(std::string protoName, const char* buffer, int len)
	{
		auto descriptor = google::protobuf::DescriptorPool::generated_pool()->FindMessageTypeByName(protoName);
		if (descriptor == nullptr)
		{
			std::cout << "协议转化异常,名称：" << protoName << std::endl;
		}
		auto type = google::protobuf::MessageFactory::generated_factory()->GetPrototype(descriptor);
		auto message = type->New();
		std::string protocol(buffer, buffer + len);
		message->ParseFromString(protocol);
		return std::make_shared<T>(*reinterpret_cast<T*>(message));
	}

	/// <summary>
	/// 按照协议名称解码
	/// </summary>
	template<typename T>
	static std::shared_ptr<T> DecodeByName(const char* buffer, int len)
	{
		try {
			int headLen = ((unsigned char)buffer[1] << 8) | buffer[0];
			std::string headName(buffer + 2, buffer + 2 + headLen);
			std::cout << "函数协议明:" << headName << std::endl;
			std::string body(buffer + 2 + headLen, buffer + len);
			auto descriptor = google::protobuf::DescriptorPool::generated_pool()->FindMessageTypeByName(headName);
			auto protoType = google::protobuf::MessageFactory::generated_factory()->GetPrototype(descriptor);
			auto instance = protoType->New();
			instance->ParseFromString(body);
			return std::make_shared<T>(*reinterpret_cast<T*>(instance));
		}
		catch (const char* error)
		{
			std::cout << "解析协议错误:" << error << std::endl;
		}
		return nullptr;
	}

	/// <summary>
	/// 根据协议编号编码
	/// </summary>
	template<class T>
	static char* Encode(ProtoID protoID, T message, int& len)
	{
		std::string str;
		message.SerializeToString(&str);
		 len = str.length()+4;
		int bodyLen = str.length();
		char* data = new char[len];
		int id = (int)protoID;
		data[0] = ( char)((len-2) % 256);
		data[1] = ( char)((len-2) / 256);
		data[2] = ( char)(id % 256);
		data[3] = ( char)(id / 256);
		memcpy(data +4, str.c_str(), bodyLen);
		return data;
	}

	/// <summary>
	/// 按照协议名称编码
	/// </summary>
	template<typename T>
	static char* EncodeByName(T msg, int& dataLen)noexcept
	{
		auto descriptor = msg.GetDescriptor();

		auto protoType = google::protobuf::MessageFactory::generated_factory()->GetPrototype(descriptor);
		std::string body = "";
		std::string name = descriptor->name();
		name = "Protocol." + name;
		msg.SerializeToString(&body);
		int nameLen = name.length();
		int bodyLen = body.length();
		dataLen = nameLen + bodyLen + 2 + 2;
		char* data = new char[dataLen];
		data[dataLen] = '\0';
		std::cout << "DataLen" << dataLen << std::endl;
		data[1] = (char)((dataLen - 2) / 256);
		data[0] = (char)((dataLen - 2) % 256);
		data[3] = (char)(nameLen / 256);
		data[2] = (char)(nameLen % 256);
		memcpy(data + 4, name.c_str(), nameLen);
		memcpy(data + 4 + nameLen, body.c_str(), bodyLen);
		std::cout << "Name:" << name << " Body:" << body << std::endl;
		return data;
	}
	/*template<>
	static std::shared_ptr<Protocol::C2SLogin> Decode(const char* buffer, int offset, int len)
	{
		std::cout << "特化版本\n";
		int headLen = ((unsigned char)buffer[1] << 8) | buffer[0];
		std::string headName(buffer + 2, buffer+2 + headLen);
		std::cout << "函数协议明:" << headName << std::endl;
		std::string body(buffer + 2 + headLen, buffer + len);
		auto descriptor = google::protobuf::DescriptorPool::generated_pool()->FindMessageTypeByName(headName);
		auto protoType = google::protobuf::MessageFactory::generated_factory()->GetPrototype(descriptor);
		auto instance = protoType->New();
		instance->ParseFromString(body);
		return std::make_shared<Protocol::C2SLogin>(*reinterpret_cast<Protocol::C2SLogin*>(instance));
	}*/

	/*static std::shared_ptr<Protocol::C2SLogin> Decode(const char* buffer, int offset, int len)
	{
		int headLen = ((unsigned char)buffer[1] << 8) | buffer[0];
		std::string headName(buffer + 2, buffer + headLen);
		std::string body(buffer + 2 + headLen, buffer + len);
		auto descriptor = google::protobuf::DescriptorPool::generated_pool()->FindMessageTypeByName(headName);
		auto protoType = google::protobuf::MessageFactory::generated_factory()->GetPrototype(descriptor);
		auto instance = protoType->New();
		instance->ParseFromString(body);
		return std::make_shared<Protocol::C2SLogin>(*static_cast<Protocol::C2SLogin*>(instance));
	}*/

	static void Test()
	{
		std::cout << "HelloTest\n";
	}


};

