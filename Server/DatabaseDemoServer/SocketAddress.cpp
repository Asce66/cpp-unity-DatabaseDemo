#include "SocketAddress.h"

SocketAddress::SocketAddress(UINT32 address, UINT16 port)
{
	GetSocketPointer()->sin_family = AF_INET;
	memset(GetSocketPointer()->sin_zero, 0, sizeof(GetSocketPointer()->sin_zero));
	GetSocketPointer()->sin_addr.S_un.S_addr = htonl(address);
	GetSocketPointer()->sin_port = htons(port);
}

SocketAddress::SocketAddress(const sockaddr& inSockAddr)
{
	//sockaddr是结构体,所以直接使用memcpy
	memcpy(&myAddr, &inSockAddr, sizeof(sockaddr));
}

size_t SocketAddress::Size()const
{
	return sizeof(sockaddr);
}

sockaddr_in* SocketAddress::GetSocketPointer()
{
	//reinterpret_cast:类型转化,即便二者没有必要联系
	return reinterpret_cast<sockaddr_in*>(&myAddr);
}
