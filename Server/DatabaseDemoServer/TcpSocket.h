#pragma once
#include<WinSock2.h>//socketx���ͷ�ļ�(ע����2)
#include<WS2tcpip.h>//��ַת��ͷ�ļ�
#pragma comment(lib, "ws2_32.lib")//ʹ��WinSock2����ʹ��
#include<string>
#include<memory>
#include<iostream>
#include"CharArray.h"
class TcpSocket
{
public:
	std::string userID;

	std::shared_ptr<CharArray> readBuffer;
	typedef std::shared_ptr<TcpSocket>tcpSocketPtr;

	SOCKET mSocket;
	sockaddr_in addr;
	int af;

	void InitBuffer(int size = CharArray::DEFAULT_SIZE);
	void InitBuffer(char* data, int len);

	TcpSocket(int af = AF_INET);
	TcpSocket(SOCKET& sock, sockaddr_in& addr_in) :mSocket(sock), addr(addr_in) {}
	~TcpSocket();

	bool Bind(const std::string& address, int port);

	void Connect(const sockaddr* addr);

	void Listen(int backBlock);

	tcpSocketPtr Accept();

	int Send(std::string& message);

	int Send(char* data, int len);

	int Receive(char* buf, int len);

};

