#pragma once
#include<WinSock2.h>//socketx相关头文件(注意是2)
#include<WS2tcpip.h>//地址转换头文件
#pragma comment(lib, "ws2_32.lib")//使得WinSock2可以使用
class SocketAddress
{
public:
	SocketAddress(UINT32 address, UINT16 port);
	SocketAddress(const sockaddr& inSockAddr);
	size_t Size()const;
private:
	sockaddr myAddr;
	sockaddr_in* GetSocketPointer();
};

