#pragma once
#include<WinSock2.h>//socketx���ͷ�ļ�(ע����2)
#include<WS2tcpip.h>//��ַת��ͷ�ļ�
#pragma comment(lib, "ws2_32.lib")//ʹ��WinSock2����ʹ��
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

