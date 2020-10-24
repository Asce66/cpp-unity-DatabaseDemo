#pragma once
#include<vector>
#include"TcpSocket.h"
#include<WinSock2.h>//socketx相关头文件(注意是2)
class TCPSelectUtil {
	typedef std::shared_ptr<TcpSocket> socketPtr;
	typedef const std::vector<socketPtr> cosVecPtr;
	typedef std::vector<socketPtr> VecPtr;
private:
	static void Fd_Set2Vector(const fd_set& inSet, cosVecPtr* inVector, VecPtr* outVector)
	{
		if (inVector && outVector)
		{
			outVector->clear();
			for (auto& p : *inVector)
			{
				if (FD_ISSET(p->mSocket, &inSet))
				{
					outVector->push_back(p);
				}
			}
		}
	}

	static void Vector2Fd_set(cosVecPtr* inVector, fd_set& outSet)
	{
		if (inVector != nullptr)
		{
			for (auto& p : *inVector)
			{
				FD_SET(p->mSocket, &outSet);
			}
		}
	}
public:
	static int Select(cosVecPtr* checkReadSockets, VecPtr* readableSockets,
		cosVecPtr* checkWriteSocket, VecPtr* writableSockets,
		cosVecPtr* checkExceptSockets, VecPtr* exceptbleSockets, const timeval* timeout = nullptr
	)
	{
		fd_set read;
		FD_ZERO(&read);//必须要归0,初始给的fd_ser内部有很多的元素
		fd_set write;
		FD_ZERO(&write);
		fd_set except;
		FD_ZERO(&except);
		Vector2Fd_set(checkReadSockets, read);
		Vector2Fd_set(checkWriteSocket, write);
		Vector2Fd_set(checkExceptSockets, except);
		int res = select(0, &read, &write, &except, timeout);
		int err = WSAGetLastError();
		if (err != 0)
			std::cout << "SelectError: " << err << std::endl;
		if (res > 0)
		{
			Fd_Set2Vector(read, checkReadSockets, readableSockets);
			Fd_Set2Vector(write, checkWriteSocket, writableSockets);
			Fd_Set2Vector(except, checkExceptSockets, exceptbleSockets);
		}
		return res;
	}
};