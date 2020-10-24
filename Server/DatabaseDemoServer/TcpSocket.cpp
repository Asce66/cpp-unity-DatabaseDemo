#include "TcpSocket.h"
typedef std::shared_ptr<TcpSocket>tcpSocketPtr;

void TcpSocket::InitBuffer(int size)
{
	readBuffer.reset (new CharArray(size));
}

void TcpSocket::InitBuffer(char* data, int len)
{
	readBuffer.reset(new CharArray(data, len));
}

TcpSocket::TcpSocket(int af)
{
	this->af = af;
	mSocket = socket(af, SOCK_STREAM, IPPROTO_TCP);
	if (mSocket == INVALID_SOCKET)
	{
		std::cout << "��ʼ��socket�쳣:" << GetLastError() << std::endl;
	}
}

TcpSocket::~TcpSocket()
{
	//std::cout << "Delete TcpSocket\n";
}

bool TcpSocket::Bind(const std::string& address, int port)
{
	memset(addr.sin_zero, 0, sizeof(addr.sin_zero));
	addr.sin_port = htons(port);
	addr.sin_family = af;
	inet_pton(af, address.c_str(), &addr.sin_addr);
	int err = bind(mSocket, reinterpret_cast<sockaddr*>(&addr), sizeof(addr));
	if (err == -1)
	{
		std::cout << "bind�쳣:" << address << " Error:" << GetLastError() << std::endl;
		return false;
	}
	return true;
}

void TcpSocket::Connect(const sockaddr* addr)
{
	int len = sizeof(*addr);
	if (connect(mSocket, addr, len) != 0)
	{
		std::cout << "connect�쳣:" << GetLastError() << std::endl;
	}
}

void TcpSocket::Listen(int backBlock)
{
	if (listen(mSocket, backBlock) == -1)
	{
		std::cout << "Listen�쳣:" << GetLastError() << std::endl;
	}
}

tcpSocketPtr TcpSocket::Accept()
{
	sockaddr_in addr_in;
	memset(addr_in.sin_zero, 0, sizeof(addr_in.sin_zero));
	int addrLen = sizeof(addr);//����Ҫ��ֵ,��Ȼ���ָ�����
	SOCKET client = accept(mSocket, reinterpret_cast<sockaddr*>(&addr_in), &addrLen);
	if (client == INVALID_SOCKET)
	{
		std::cout << "accept�쳣��" << GetLastError() << std::endl;
		return nullptr;
	}
	std::cout << "�������˿ͻ���:\n";
	return (tcpSocketPtr(new TcpSocket(client, addr_in)));
}

int TcpSocket::Send(std::string& message)
{
	int len = send(mSocket, message.c_str(), message.length() + 1, 0);
	if (len == -1)
	{
		std::cout << "send�쳣:" << GetLastError() << std::endl;
		return -1;
	}
	return len;
}

int TcpSocket::Send(char* data, int len)
{
	int res = send(mSocket, data, len, 0);
	if (res == -1) {

		std::cout << "���������쳣:" << WSAGetLastError() << std::endl;
		return -1;
	}
	return res;
}

int TcpSocket::Receive(char* buf, int len)
{
	int recLen = recv(mSocket, buf, len, 0);
	if (recLen == -1)
	{
		std::cout << "���������쳣:" << GetLastError() << std::endl;
		return -1;
	}
	return recLen;

}
