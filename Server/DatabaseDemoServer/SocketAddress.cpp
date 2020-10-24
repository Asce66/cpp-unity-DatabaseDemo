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
	//sockaddr�ǽṹ��,����ֱ��ʹ��memcpy
	memcpy(&myAddr, &inSockAddr, sizeof(sockaddr));
}

size_t SocketAddress::Size()const
{
	return sizeof(sockaddr);
}

sockaddr_in* SocketAddress::GetSocketPointer()
{
	//reinterpret_cast:����ת��,�������û�б�Ҫ��ϵ
	return reinterpret_cast<sockaddr_in*>(&myAddr);
}
