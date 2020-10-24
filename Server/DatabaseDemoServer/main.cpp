#include<iostream>
#include<WinSock2.h>//socketx相关头文件(注意是2)
#include<WS2tcpip.h>//地址转换头文件
#pragma comment(lib, "ws2_32.lib")//使得WinSock2可以使用

#include"SocketAddress.h"
#include<string>
#include<memory>
#include<cstring>

#include"MsgUser.pb.h"
#include"ProtobufParser.h"
# include"TCPSelectUtil.h"
#include"TcpSocket.h"
#include"CharArray.h"

#include"mysql.h"
#include<map>
#include "DBManager.h"
#include"Handler.h"
#include<cstring>

//#pragma comment(lib, "libmysql.lib")

/// <summary>
/// socket有关函数介绍
/// </summary>
int socketFunc()
{
	WSADATA data;
	/// <summary>
	/// 激活socket库
	/// 参数一:2个字节的WORD(ushort),低字节表示主版本号,高字节表示所需WinSock实现的最低版本
	/// 参数二:函数填入被激活的socket库信息,如实现的版本
	/// </summary>
	/// <returns>0:正常  其他:错误原因</returns>
	int err = WSAStartup(MAKEWORD(2, 2), &data);
	if (err != 0)
	{
		std::cout << "初始化失败";
		return 0;
	}

	/// <summary>
	/// 创建一个socket
	/// 参数一:协议族,指socket使用的网络协议  AF_INET(IPv4) AF_INET6(IPv6)
	/// 参数二:数据传输形式  SOCK_STREAM(有序可靠的分段流) SOCK_DGRAM(离散报文)
	/// 参数三:传输协议 IPPROTO_TCP(tcp)  IPPROTO_UDP(udp) 0(根据数据形式(参数二)指定合适协议)
	/// </summary>	 
	SOCKET sock = socket(AF_INET, SOCK_DGRAM, IPPROTO_UDP);

	/// <summary>
	/// 返回此线程中最近一次的错误代码
	/// socket函数如果出现错误就会返回-1,windows中使用宏SOCKET_ERROR代替
	/// 但是一个-1不能获得准确的错误信息,所以使用此函数
	/// 此函数只获得最近的错误,如果发生一个错误后可能会连环造成其他错误发生
	/// 就无法获得源头错误,因此应当在socket函数返回-1时就立即调用此函数
	/// </summary>
	WSAGetLastError();

	/// <summary>
	/// 停止传输消息,用于tcp连接的断开前使用
	/// 参数二:SD_SEND(停止发送消息,并会发送出一个FIN)  SD_RECEIVE(停止接受消息)  SD_BOTH(都停止)
	/// </summary>
	shutdown(sock, SD_BOTH);

	//关闭socket
	closesocket(sock);

	//关闭socket库,释放资源(会结束所有socket的操作,确保socket都关闭或未使用)
	//WSAStartup()激活时采用的引用计数,即可多次激活,所以关闭的次数必须与激活次数一致
	WSACleanup();

	return 0;
}

/// <summary>
/// socket地址有关函数介绍
/// </summary>
void socketAdd()
{
	//socket地址
	sockaddr add;
	add.sa_family;//地址类型,应该与socket创建时的网络协议一致(ipv4,ipv6等)
	add.sa_data;//存储真正地址

	//地址初始化专业数据类型
	//socketAPI建立时没有类和多态继承,所以在socket函数需要
	//地址数据时必须手动把这个数据转为sockaddr
	sockaddr_in addr_in;
	addr_in.sin_family;//与sockaddr一样
	addr_in.sin_port;//16位端口号
	addr_in.sin_addr;//地址
	addr_in.sin_zero;//不使用,用于填补大小与sockaddr大小一致,所以全设为0

	//主机字节序与网络字节序之间转化，因为tcp/ip协议族可能和主机在多字节数的字节序上标准不同(即网络中的大端小端问题)
	htons(88);//主机->网络(uint16)
	htonl(66);//主机->网络(uint32)
	ntohs(88);//网络->主机(uint16)
	ntohl(66);//网络->主机(uint32)

	//绑定socket
	//int bind(SOCKET socket,const sockaddr*add,int add_len);
	//成功返回0,错误返回-1

	//example:初始化一个sockaddr_in
	sockaddr_in myAdd;
	memset(myAdd.sin_zero, 0, sizeof(myAdd.sin_zero));
	myAdd.sin_family = AF_INET;
	myAdd.sin_port = htons(88);
	myAdd.sin_addr.S_un.S_un_b.s_b1 = 192;
	myAdd.sin_addr.S_un.S_un_b.s_b2 = 168;
	myAdd.sin_addr.S_un.S_un_b.s_b3 = 1;
	myAdd.sin_addr.S_un.S_un_b.s_b4 = 9;
	//直接把字符串转化为地址
	inet_pton(AF_INET, "192.168.1.9", &myAdd.sin_addr);
}

inline uint16_t ByteSwap2(uint16_t data)
{
	return (data >> 8) | (data << 8);
}

inline uint32_t ByteSwap4(uint32_t data)
{
	return ((data >> 24) & 0x000000ff) |
		((data >> 8) & 0x0000ff00) |
		((data << 8) & 0x00ff0000) |
		((data << 24) & 0xff000000);
}

template<typename tFrom, typename tTo>
struct TypeAliser {
	union {
		tFrom fromVal;
		tTo toVal;
	};
	TypeAliser(tFrom data) :fromVal(data) {}
	tTo& get() { return toVal; }
};

template<typename tFrom, size_t size>
struct SwapBuffeer;

template<typename T>
struct SwapBuffeer<T, 4>
{
	T Swap(T indata)const
	{
		uint32_t result = ByteSwap4(TypeAliser<T, uint32_t>(indata).get());
		return TypeAliser<uint32_t, T>(result).get();
	}
};

struct TestU
{
	union {
		int aa;
		float bb;
	};
	TestU(int a) :aa(a) {}
	void Print() { std::cout << bb; }
};
using namespace std;

/// <summary>
/// select的相关说明
/// </summary>
void selectFunc()
{
	fd_set read_set;//存放待检查的socket
	FD_ZERO(&read_set);//赋值为0
	//FD_SET(socket, read_set);//向fd_set添加一个socket
	//FD_ISSET(socket, &read_set);//检查fd_set里面有没有指定的socket

	//参数一:在POSIX(可移植)平台下是代表检查的socket列表里的最大编号
	//POSIX下所有socket都是整数,所以直接选择socket的最大值即可
	//Windows下socket是指针,所以没啥用
	//参数四:超时前可等待的最长时间指针,nullptr表示不等待
	//如果任意socket检查时超时,停止其他socket的检查,并且所有检查列表的是返回的空
	//返回值:所有检查列表的剩余socket总数
	//select(0,)
}

typedef std::shared_ptr<TcpSocket>TcpSockPtr;

unordered_map<int, unique_ptr<HandleBase>>mm;

vector<TcpSockPtr>tcpClientVec;
vector<TcpSockPtr>readableVec;

void ReceiveConnect(TcpSockPtr client)
{
	client->InitBuffer();
	tcpClientVec.push_back(client);
	//cout << "accept!!\n";
}

void ProcessMessage(TcpSockPtr client)
{
	while (client->readBuffer->Length() > 2)
	{
		int len = client->readBuffer->GetInt();
		if (client->readBuffer->Length() - 2 < len)
			break;
		client->readBuffer->readIndex += 2;
		int protoID = client->readBuffer->GetInt();
		client->readBuffer->readIndex += 2;
		if (mm.find(protoID) == mm.end())
		{
			std::cout << "handler不存在:" << protoID << std::endl;
			return;
		}
		mm[protoID]->HandleMessage(client, client->readBuffer->data + client->readBuffer->readIndex, len - 2);
		client->readBuffer->readIndex += len - 2;
		client->readBuffer->CheckAndMove();
	}
	client->readBuffer->CheckAndMove();
}

void ReceiveMeesage(TcpSockPtr client)
{
	int len = recv(client->mSocket, client->readBuffer->data, client->readBuffer->capacity, 0);
	if (len == 0)
	{
		shutdown(client->mSocket, SD_BOTH);
		closesocket(client->mSocket);
		cout << "一位客户端断开连接\n";
		tcpClientVec.erase(find(tcpClientVec.begin(), tcpClientVec.end(), client));
		cout << "现在客户端数目:" << tcpClientVec.size() << endl;
	}
	else
	{
		std::cout << "收到消息，长度为:" << len << std::endl;
		client->readBuffer->writeIndex += len;
		ProcessMessage(client);
	}
}

int main()
{
	//初始化消息处理对象
	mm[(int)ProtoID::C2SLogin] = std::make_unique<C2SLoginHandler>(C2SLoginHandler());
	mm[(int)ProtoID::C2SWriter] = std::make_unique< C2SWriterHandler>(C2SWriterHandler());
	mm[(int)ProtoID::C2SAddWriters] = std::make_unique<C2SAddWritersHandler>(C2SAddWritersHandler());
	mm[(int)ProtoID::C2SWriteBook] = std::make_unique<C2SWriteBookHandler>(C2SWriteBookHandler());
	mm[(int)ProtoID::C2SAddWriteBook] = std::make_unique<C2SAddWriteBookHandler>(C2SAddWriteBookHandler());
	mm[(int)ProtoID::C2SDoWeiWen] = std::make_unique<C2SDoWeiWenHandler>(C2SDoWeiWenHandler());
	mm[(int)ProtoID::C2SAddDoWeiWen] = std::make_unique< C2SAddDoWeiWenHandler>(C2SAddDoWeiWenHandler());
	mm[(int)ProtoID::C2SDoYanTao] = std::make_unique<C2SDoYanTaoHandler>(C2SDoYanTaoHandler());
	mm[(int)ProtoID::C2SAddDoYanTao] = std::make_unique<C2SAddDoYanTaoHandler>(C2SAddDoYanTaoHandler());
	mm[(int)ProtoID::C2SDeleteWriters] = std::make_unique<C2SDeleteWritersHandler>(C2SDeleteWritersHandler());
	mm[(int)ProtoID::C2SUpdateWriters] = std::make_unique<C2SUpdateWritersHandler>(C2SUpdateWritersHandler());
	mm[(int)ProtoID::C2SBook] = std::make_unique<C2SBookHandler>(C2SBookHandler());
	mm[(int)ProtoID::C2SAddBooks] = std::make_unique<C2SAddBooksHandler>(C2SAddBooksHandler());
	mm[(int)ProtoID::C2SUpdateBooks] = std::make_unique<C2SUpdateBooksHandler>(C2SUpdateBooksHandler());
	mm[(int)ProtoID::C2SDeleteBooks] = std::make_unique<C2SDeleteBooksHandler>(C2SDeleteBooksHandler());
	mm[(int)ProtoID::C2SAddYanTao] = std::make_unique<C2SAddYanTaoHandler>(C2SAddYanTaoHandler());
	mm[(int)ProtoID::C2SDeleteYanTao] = std::make_unique<C2SDeleteYanTaoHandler>(C2SDeleteYanTaoHandler());
	mm[(int)ProtoID::C2SUpdateYanTao] = std::make_unique<C2SUpdateYanTaoHandler>(C2SUpdateYanTaoHandler());
	mm[(int)ProtoID::C2SYanTao] = std::make_unique<C2SYanTaoHandler>(C2SYanTaoHandler());
	mm[(int)ProtoID::C2SDepartment] = std::make_unique<C2SDepartmentHandler>(C2SDepartmentHandler());
	mm[(int)ProtoID::C2SAddDepartment] = std::make_unique<C2SAddDepartmentHandler>(C2SAddDepartmentHandler());
	mm[(int)ProtoID::C2SUpdateDepartment] = std::make_unique<C2SUpdateDepartmentHandler>(C2SUpdateDepartmentHandler());
	mm[(int)ProtoID::C2SDeleteDepartment] = std::make_unique<C2SDeleteDepartmentHandler>(C2SDeleteDepartmentHandler());
	mm[(int)ProtoID::C2SWeiWen] = std::make_unique<C2SWeiWenHandler>(C2SWeiWenHandler());
	mm[(int)ProtoID::C2SAddWeiWen] = std::make_unique<C2SAddWeiWenHandler>(C2SAddWeiWenHandler());
	mm[(int)ProtoID::C2SUpdateWeiWen] = std::make_unique<C2SUpdateWeiWenHandler>(C2SUpdateWeiWenHandler());
	mm[(int)ProtoID::C2SDeleteWeiWen] = std::make_unique<C2SDeleteWeiWenHandler>(C2SDeleteWeiWenHandler());
	mm[(int)ProtoID::C2SSearchDoWeiwen] = std::make_unique<C2SSearchDoWeiWenHandler>(C2SSearchDoWeiWenHandler());
	mm[(int)ProtoID::C2SSearchWriteBook] = std::make_unique<C2SSearchWriteBookHandler>(C2SSearchWriteBookHandler());
	mm[(int)ProtoID::C2SSearchDoYanTao] = std::make_unique<C2SSearchDoYanTaoHandler>(C2SSearchDoYanTaoHandler());
	mm[(int)ProtoID::C2SRegister] = std::make_unique< C2SRegisterHandler>(C2SRegisterHandler());

	//数据库连接
	DBManager& dbManager = DBManager::Instance();
	dbManager.Connect();

	//创建并绑定socket
	WSADATA data;
	int err = WSAStartup(MAKEWORD(2, 2), &data);
	if (err != 0)
	{
		cout << "初始化错误\n";
		return 0;
	}

	TcpSockPtr tcpServerPtr = make_shared<TcpSocket>(TcpSocket());
	string ipAddress = "";
	bool isBindOk = false;
	do {
		cout << "请输入你的服务器IP地址\n";
		cin >> ipAddress;
		isBindOk = tcpServerPtr->Bind(ipAddress, 8866);//"43.226.145.241"
		if (isBindOk) {
			tcpServerPtr->Listen(100);
			tcpClientVec.push_back(tcpServerPtr);
		}
	} while (isBindOk == false);
	cout << "*************************服务器已启动*************************\n";
	while (true)
	{
		int res = TCPSelectUtil::Select(&tcpClientVec, &readableVec, nullptr, nullptr, nullptr, nullptr);
		if (res > 0)
		{
			for (auto& p : readableVec)
			{
				if (p == tcpServerPtr)
				{
					TcpSockPtr client = tcpServerPtr->Accept();
					ReceiveConnect(client);
				}
				else
				{
					ReceiveMeesage(p);
				}
			}
		}
	}

	int i = WSAGetLastError();
	if (i != 0)
		cout << "Error:" << i << endl;
	WSACleanup();
	cout << "*************************服务器已关闭*************************\n";
	system("pause");
	return 0;
}