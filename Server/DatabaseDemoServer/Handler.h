#pragma once
#include"TcpSocket.h"
#include"MsgData.pb.h"
#include"MsgUser.pb.h"
#include"ProtobufParser.h"
#include"DBManager.h"
#include<iostream>
using namespace std;

const string protoName[] = { "C2SLogin","S2CLogin","C2SRegister","S2CRegister",
"C2SWriter","S2CWriter","C2SAddWriters","S2CAddWriters","C2SBook","S2CBook","C2SAddBooks","C2SDeleteBooks",
"UpdateBook","C2SUpdateBooks","C2SYanTao","S2CYanTao","C2SWeiWen","S2CWeiWen",
"C2SWriteBook","S2CWriteBook","C2SDoYanTao","S2CDoYanTao","C2SDoWeiWen","S2cDoWeiWen","C2SAddWriteBook","C2SAddDoWeiWen","C2SAddDoYanTao",
"C2SDeleteWriters","C2SUpdateWriters","C2SAddYanTao","UpdateYanTao","C2SUpdateYanTao","C2SDeleteYanTao", "C2SDepartment",
	"S2CDepartment","C2SAddDepartment","UpdateDepartment","C2SUpdateDepartment","C2SDeleteDepartment","C2SAddWeiWen",
	"UpdateWeiWen","C2SUpdateWeiWen","C2SDeleteWeiWen", "C2SSearchDoYanTao","S2CSearchDoYanTao","UpdateDoYanTao",
	"C2SUpdateDoYanTao","C2SDeleteDoYanTao","C2SSearchWriteBook","S2CSearchWriteBook","C2SSearchDoWeiwen","S2CSearchDoWeiwen"
};

typedef std::shared_ptr<TcpSocket>TcpSockPtr;

class HandleBase {
public:
	virtual void HandleMessage(TcpSockPtr sockPtr, char* buffer, int len) = 0;
	virtual ~HandleBase() {}
};

class C2SLoginHandler :public HandleBase {
public:
	void HandleMessage(TcpSockPtr sockPtr, char* buffer, int len)
	{
		try {
			std::shared_ptr<Protocol::C2SLogin>c2sptr = ProtobufParser::Decode<Protocol::C2SLogin>("Protocol." + protoName[(int)ProtoID::C2SLogin], buffer, len);
			//cout << "用户：" << c2sptr->account() << "   密码：" << c2sptr->password() << endl;
			DBManager& dbmnr = DBManager::Instance();
			Protocol::S2CLogin response;
			response.set_result(dbmnr.CheckUser(c2sptr->account(), c2sptr->password()));
			//cout << "登录结果：" << response.result() << endl;
			if (response.result() == 1)
				sockPtr->userID = c2sptr->account();
			len = 0;
			char* data = ProtobufParser::Encode(ProtoID::S2CLogin, response, len);
			sockPtr->Send(data, len);
		}
		catch (const char* str)
		{
			cout << "LoginHandler异常:" << str << endl;
		}
	}
};
class C2SRegisterHandler :public HandleBase {
public:
	void HandleMessage(TcpSockPtr sockPtr, char* buffer, int len)
	{
		std::shared_ptr<Protocol::C2SRegister> request = ProtobufParser::Decode<Protocol::C2SRegister>
			("Protocol." + protoName[ProtoID::C2SRegister], buffer, len);
		bool isExist = DBManager::Instance().IsExistUser(request->account());
		Protocol::S2CRegister response;
		response.set_result(0);
		if (isExist == false)
		{
			DBManager::Instance().AddUser(request->account(), request->password());
			response.set_result(1);
		}
		char* data = ProtobufParser::Encode(ProtoID::S2CRegister, response, len);
		sockPtr->Send(data, len);
	}
};

class C2SWriterHandler :public HandleBase {
public:
	void HandleMessage(TcpSockPtr sockPtr, char* buffer, int len)
	{
		string id = sockPtr->userID;
		vector<Protocol::Writer>writers = DBManager::Instance().SearchWriterList(id);
		Protocol::S2CWriter response;
		for (int i = 0; i < writers.size(); ++i)
		{
			auto pp = response.add_writerlist();
			*pp = writers[i];
		}
		len = 0;
		char* data = ProtobufParser::Encode(ProtoID::S2CWriter, response, len);
		sockPtr->Send(data, len);
	}
};

class C2SAddWritersHandler :public HandleBase {
public:
	void HandleMessage(TcpSockPtr sockPtr, char* buffer, int len)
	{
		std::shared_ptr<Protocol::C2SAddWriters>c2sAddWriterPtr = ProtobufParser::Decode
			<Protocol::C2SAddWriters>("Protocol." + protoName[(int)ProtoID::C2SAddWriters], buffer, len);
		int size = c2sAddWriterPtr->writerlist_size();
		vector<Protocol::Writer>writers(size);
		for (int i = 0; i < size; ++i)
		{
			writers[i] = c2sAddWriterPtr->writerlist(i);
		}
		bool result = DBManager::Instance().AddWriters(writers, sockPtr->userID);
		Protocol::S2CAddWriters response;
		response.set_result(result);
		char* data = ProtobufParser::Encode(ProtoID::S2CAddWriters, response, len);
		sockPtr->Send(data, len);
	}
};
class C2SWriteBookHandler :public HandleBase {
public:
	void HandleMessage(TcpSockPtr sockPtr, char* buffer, int len)
	{
		vector<Protocol::Book>vec = DBManager::Instance().SearchBooks(sockPtr->userID);
		Protocol::S2CWriteBook respose;
		for (int i = 0; i < vec.size(); ++i)
		{
			auto pp = respose.add_writebooklist();
			*pp = vec[i];
		}
		char* data = ProtobufParser::Encode(ProtoID::S2CWriteBook, respose, len);
		sockPtr->Send(data, len);

	}
};

class C2SAddWriteBookHandler :public HandleBase {
public:
	void HandleMessage(TcpSockPtr sockPtr, char* buffer, int len)
	{
		shared_ptr<Protocol::C2SAddWriteBook> request =
			ProtobufParser::Decode<Protocol::C2SAddWriteBook>("Protocol." + protoName[(int)ProtoID::C2SAddWriteBook], buffer, len);
		vector<Protocol::WriteBook>vec(request->writebooklist_size());
		for (int i = 0; i < vec.size(); ++i)
		{
			vec[i] = request->writebooklist(i);
			std::cout << "WriteBook " << vec[i].bbh() << "  " << vec[i].sbh() << std::endl;
		}
		DBManager::Instance().AddWriteBook(sockPtr->userID, vec);
	}
};
class C2SDoWeiWenHandler :public HandleBase {
public:
	void HandleMessage(TcpSockPtr sockPtr, char* buffer, int len)
	{
		std::vector<Protocol::WeiWen>vec = DBManager::Instance().SearchWeiWen(sockPtr->userID);
		Protocol::S2cDoWeiWen response;
		for (int i = 0; i < vec.size(); ++i)
		{
			auto p = response.add_doweiwenlist();
			*p = vec[i];
		}
		char* data = ProtobufParser::Encode(ProtoID::S2cDoWeiWen, response, len);
		sockPtr->Send(data, len);
	}
};

class C2SAddDoWeiWenHandler :public HandleBase {
public:
	void HandleMessage(TcpSockPtr sockPtr, char* buffer, int len)
	{
		std::shared_ptr<Protocol::C2SAddDoWeiWen>request = ProtobufParser::Decode<Protocol::C2SAddDoWeiWen>
			("Protocol." + protoName[(int)ProtoID::C2SAddDoWeiWen], buffer, len);
		std::vector<Protocol::DoWeiWen>vec(request->doweiwenlist_size());
		for (int i = 0; i < request->doweiwenlist_size(); ++i)
		{
			vec[i] = request->doweiwenlist(i);
		}
		DBManager::Instance().AddDoWeiWen(sockPtr->userID, vec);
	}
};

class C2SDoYanTaoHandler :public HandleBase {
public:
	void HandleMessage(TcpSockPtr sockPtr, char* buffer, int len)
	{
	std:; vector<Protocol::YanTao>vec = DBManager::Instance().SearchYanTao(sockPtr->userID);
		Protocol::S2CDoYanTao response;
		for (int i = 0; i < vec.size(); ++i)
		{
			auto p = response.add_doyantaolist();
			*p = vec[i];
		}
		char* data = ProtobufParser::Encode(ProtoID::S2CDoYanTao, response, len);
		sockPtr->Send(data, len);
	}
};

class C2SAddDoYanTaoHandler :public HandleBase {
public:
	void HandleMessage(TcpSockPtr sockPtr, char* buffer, int len)
	{
		shared_ptr<Protocol::C2SAddDoYanTao>request = ProtobufParser::Decode
			<Protocol::C2SAddDoYanTao>("Protocol." + protoName[(int)ProtoID::C2SAddDoYanTao], buffer, len);
		std::vector<Protocol::DoYanTao>vec(request->doyantaolist_size());
		for (int i = 0; i < request->doyantaolist_size(); ++i)
		{
			vec[i] = request->doyantaolist(i);
		}
		DBManager::Instance().AddDoYanTao(sockPtr->userID, vec);
	}
};
class C2SDeleteWritersHandler :public HandleBase {
public:
	void HandleMessage(TcpSockPtr sockPtr, char* data, int len)
	{
		shared_ptr<Protocol::C2SDeleteWriters>request = ProtobufParser::Decode<Protocol::C2SDeleteWriters>
			("Protocol." + protoName[(int)ProtoID::C2SDeleteWriters], data, len);
		vector<string>vec(request->writerlist_size());
		for (int i = 0; i < request->writerlist_size(); ++i)
		{
			vec[i] = request->writerlist(i);
		}
		DBManager::Instance().DeleteDataByString("delete from writer where BH='%s' and id='%s'", sockPtr->userID, vec);
	}
};

class C2SUpdateWritersHandler :public HandleBase {
public:
	void HandleMessage(TcpSockPtr sockPtr, char* data, int len)
	{
		shared_ptr<Protocol::C2SUpdateWriters>request = ProtobufParser::Decode<Protocol::C2SUpdateWriters>
			("Protocol." + protoName[(int)ProtoID::C2SUpdateWriters], data, len);
		vector<Protocol::UpdateWriter>vec(request->writerlist_size());
		for (int i = 0; i < request->writerlist_size(); ++i)
		{
			vec[i] = request->writerlist(i);
		}
		DBManager::Instance().UpdateWriters(sockPtr->userID, vec);
	}
};

class C2SBookHandler :public HandleBase {
public:
	void HandleMessage(TcpSockPtr sockPtr, char* buffer, int len)
	{
		std::vector<Protocol::Book>books = DBManager::Instance().SearchBooks(sockPtr->userID);
		Protocol::S2CBook response;
		for (int i = 0; i < books.size(); ++i)
		{
			auto p = response.add_booklist();
			*p = books[i];
		}
		char* data = ProtobufParser::Encode(ProtoID::S2CBook, response, len);
		sockPtr->Send(data, len);
	}
};

class C2SAddBooksHandler :public HandleBase {
public:
	void HandleMessage(TcpSockPtr sockPtr, char* buffer, int len)
	{
		shared_ptr<Protocol::C2SAddBooks>request = ProtobufParser::Decode<Protocol::C2SAddBooks>
			("Protocol." + protoName[(int)ProtoID::C2SAddBooks], buffer, len);
		vector<Protocol::Book>books(request->booklist_size());
		for (int i = 0; i < request->booklist_size(); ++i)
		{
			books[i] = request->booklist(i);
		}
		DBManager::Instance().AddBooks(sockPtr->userID, books);
	}
};

class C2SUpdateBooksHandler :public HandleBase {
public:
	void HandleMessage(TcpSockPtr sockPtr, char* buffer, int len)
	{
		shared_ptr<Protocol::C2SUpdateBooks>request = ProtobufParser::Decode<Protocol::C2SUpdateBooks>
			("Protocol." + protoName[(int)ProtoID::C2SUpdateBooks], buffer, len);
		vector<Protocol::UpdateBook>vec(request->booklist_size());
		for (int i = 0; i < vec.size(); ++i)
		{
			vec[i] = request->booklist(i);
		}
		DBManager::Instance().UpdateBooks(sockPtr->userID, vec);
	}
};

class C2SDeleteBooksHandler :public HandleBase {
public:
	void HandleMessage(TcpSockPtr sockPtr, char* buffer, int len)
	{
		shared_ptr<Protocol::C2SDeleteBooks>request = ProtobufParser::Decode<Protocol::C2SDeleteBooks>
			("Protocol." + protoName[(int)ProtoID::C2SDeleteBooks], buffer, len);
		vector<string>vec(request->bhlist_size());
		for (int i = 0; i < request->bhlist_size(); ++i)
		{
			vec[i] = request->bhlist(i);
		}
		DBManager::Instance().DeleteDataByString("delete from book where BH='%s' and id='%s'", sockPtr->userID, vec);
	}
};
class C2SAddYanTaoHandler :public HandleBase {
public:
	void HandleMessage(TcpSockPtr sockPtr, char* buffer, int len)
	{
		shared_ptr<Protocol::C2SAddYanTao>request = ProtobufParser::Decode<Protocol::C2SAddYanTao>
			("Protocol." + protoName[(int)ProtoID::C2SAddYanTao], buffer, len);
		vector<Protocol::YanTao>vec(request->yantaolist_size());
		for (int i = 0; i < request->yantaolist_size(); ++i)
		{
			vec[i] = request->yantaolist(i);
		}
		DBManager::Instance().AddYanTao(sockPtr->userID, vec);
	}
};
class C2SUpdateYanTaoHandler :public HandleBase {
public:
	void HandleMessage(TcpSockPtr sockPtr, char* buffer, int len)
	{
		shared_ptr<Protocol::C2SUpdateYanTao>request = ProtobufParser::Decode<Protocol::C2SUpdateYanTao>
			("Protocol." + protoName[(int)ProtoID::C2SUpdateYanTao], buffer, len);
		vector<Protocol::UpdateYanTao>vec(request->yantaolist_size());
		for (int i = 0; i < request->yantaolist_size(); ++i)
		{
			vec[i] = request->yantaolist(i);
		}
		DBManager::Instance().UpdateYanTao(sockPtr->userID, vec);

	}
};
class C2SDeleteYanTaoHandler :public HandleBase {
public:
	void HandleMessage(TcpSockPtr sockPtr, char* buffer, int len)
	{
		shared_ptr<Protocol::C2SDeleteYanTao>request = ProtobufParser::Decode<Protocol::C2SDeleteYanTao>
			("Protocol." + protoName[(int)ProtoID::C2SDeleteYanTao], buffer, len);
		vector<string>vec(request->name_size());
		for (int i = 0; i < request->name_size(); ++i)
		{
			vec[i] = request->name(i);
		}
		DBManager::Instance().DeleteDataByString("delete from yantao where name='%s' and id='%s'", sockPtr->userID, vec);
	}
};
class C2SYanTaoHandler :public HandleBase {
public:
	void HandleMessage(TcpSockPtr sockPtr, char* buffer, int len)
	{
		vector<Protocol::YanTao>vec = DBManager::Instance().SearchYanTao(sockPtr->userID);
		Protocol::S2CYanTao response;
		for (int i = 0; i < vec.size(); ++i)
		{
			auto p = response.add_yantaolist();
			*p = vec[i];
		}
		char* data = ProtobufParser::Encode(ProtoID::S2CYanTao, response, len);
		sockPtr->Send(data, len);
	}
};

class C2SDepartmentHandler :public HandleBase {

public:
	void HandleMessage(TcpSockPtr sockPtr, char* buffer, int len)
	{
		vector<Protocol::Department>vec = DBManager::Instance().SearchDep(sockPtr->userID);
		Protocol::S2CDepartment response;
		for (int i = 0; i < vec.size(); ++i)
		{
			auto p = response.add_departmentlist();
			*p = vec[i];
		}
		char* data = ProtobufParser::Encode(ProtoID::S2CDepartment, response, len);
		sockPtr->Send(data, len);
	}
};

class C2SAddDepartmentHandler :public HandleBase {
public:
	void HandleMessage(TcpSockPtr sockPtr, char* buffer, int len)
	{
		shared_ptr<Protocol::C2SAddDepartment>request = ProtobufParser::Decode<Protocol::C2SAddDepartment>
			("Protocol." + protoName[(int)ProtoID::C2SAddDepartment], buffer, len);
		vector<Protocol::Department>vec(request->departmentlist_size());
		for (int i = 0; i < vec.size(); ++i)
		{
			vec[i] = request->departmentlist(i);
		}
		DBManager::Instance().AddDep(sockPtr->userID, vec);
	}
};

class C2SUpdateDepartmentHandler :public HandleBase {
public:
	void HandleMessage(TcpSockPtr sockPtr, char* buffer, int len)
	{
		shared_ptr<Protocol::C2SUpdateDepartment>request = ProtobufParser::Decode<Protocol::C2SUpdateDepartment>
			("Protocol." + protoName[(int)ProtoID::C2SUpdateDepartment], buffer, len);
		vector<Protocol::UpdateDepartment>vec(request->departmentlist_size());
		for (int i = 0; i < vec.size(); ++i)
		{
			vec[i] = request->departmentlist(i);
			cout << vec[i].department().name() << endl;
		}
		DBManager::Instance().UpdateDep(sockPtr->userID, vec);
	}
};

class C2SDeleteDepartmentHandler :public HandleBase {
public:
	void HandleMessage(TcpSockPtr sockPtr, char* buffer, int len)
	{
		shared_ptr<Protocol::C2SDeleteDepartment>request = ProtobufParser::Decode<Protocol::C2SDeleteDepartment>
			("Protocol." + protoName[(int)ProtoID::C2SDeleteDepartment], buffer, len);
		vector<string>vec(request->bhlist_size());
		for (int i = 0; i < vec.size(); ++i)
		{
			vec[i] = request->bhlist(i);
		}
		DBManager::Instance().DeleteDataByString("delete from department where BMH='%s' and id='%s'", sockPtr->userID, vec);
	}
};
class C2SWeiWenHandler :public HandleBase {
public:
	void HandleMessage(TcpSockPtr sockPtr, char* buffer, int len)
	{
		vector<Protocol::WeiWen>vec = DBManager::Instance().SearchWeiWen(sockPtr->userID);
		Protocol::S2CWeiWen response;
		for (int i = 0; i < vec.size(); ++i)
		{
			auto p = response.add_weiwenlist();
			*p = vec[i];
		}
		char* data = ProtobufParser::Encode(ProtoID::S2CWeiWen, response, len);
		sockPtr->Send(data, len);
	}
};
class C2SAddWeiWenHandler :public HandleBase {
public:
	void HandleMessage(TcpSockPtr sockPtr, char* buffer, int len)
	{
		shared_ptr<Protocol::C2SAddWeiWen>request = ProtobufParser::Decode<Protocol::C2SAddWeiWen>
			("Protocol." + protoName[(int)ProtoID::C2SAddWeiWen], buffer, len);
		vector<Protocol::WeiWen>vec(request->weiwenlist_size());
		for (int i = 0; i < vec.size(); ++i)
		{
			vec[i] = request->weiwenlist(i);
		}
		DBManager::Instance().AddWeiWen(sockPtr->userID, vec);
	}
};

class C2SUpdateWeiWenHandler :public HandleBase {
public:
	void HandleMessage(TcpSockPtr sockPtr, char* buffer, int len)
	{
		shared_ptr<Protocol::C2SUpdateWeiWen>request = ProtobufParser::Decode<Protocol::C2SUpdateWeiWen>
			("Protocol." + protoName[(int)ProtoID::C2SUpdateWeiWen], buffer, len);
		vector<Protocol::UpdateWeiWen>vec(request->weiwenlist_size());
		for (int i = 0; i < vec.size(); ++i)
		{
			vec[i] = request->weiwenlist(i);
		}
		DBManager::Instance().UpdateWeiWen(sockPtr->userID, vec);
	}
};

class C2SDeleteWeiWenHandler :public HandleBase {
public:
	void HandleMessage(TcpSockPtr sockPtr, char* buffer, int len)
	{
		shared_ptr<Protocol::C2SDeleteWeiWen>request = ProtobufParser::Decode<Protocol::C2SDeleteWeiWen>
			("Protocol." + protoName[(int)ProtoID::C2SDeleteWeiWen], buffer, len);
		vector<string>vec(request->name_size());
		for (int i = 0; i < vec.size(); ++i)
		{
			vec[i] = request->name(i);
		}
		DBManager::Instance().DeleteDataByString("delete from weiwen where name='%s' and id='%s'", sockPtr->userID, vec);
	}
};
class C2SSearchDoYanTaoHandler :public HandleBase {
public:
	void HandleMessage(TcpSockPtr sockPtr, char* buffer, int len)
	{
		Protocol::S2CSearchDoYanTao response;
		vector<Protocol::DoYanTao>vec = DBManager::Instance().SearchDoYanTao(sockPtr->userID);
		for (int i = 0; i < vec.size(); ++i)
		{
			auto p = response.add_doyantaolist();
			*p = vec[i];
		}
		char* data = ProtobufParser::Encode(ProtoID::S2CSearchDoYanTao, response, len);
		sockPtr->Send(data, len);
	}
};
class C2SUpdateDoYanTaoHandler :public HandleBase {
public:
	void HandleMessage(TcpSockPtr sockPtr, char* buffer, int len)
	{
		shared_ptr<Protocol::C2SUpdateDoYanTao>request = ProtobufParser::Decode<Protocol::C2SUpdateDoYanTao>("Protocol." + protoName[(int)ProtoID::C2SUpdateDoYanTao], buffer, len);
		vector<vector<string>>data(request->yantaolist_size());
		vector<string>oldData(request->yantaolist_size());
		for (int i = 0; i < data.size(); ++i)
		{
			data[i][0] = request->yantaolist(i).yantao().sbh();
			data[i][1] = request->yantaolist(i).yantao().name();
			oldData[i] = request->yantaolist(i).oldname();
		}
		DBManager::Instance().UpdateActivities("update doyantao set SBH='%s',name='%s' where SBH='%s' and id='%s'", sockPtr->userID, data, oldData);
	}
};
class C2SDeleteDoYanTaoHandler :public HandleBase {
public:
	void HandleMessage(TcpSockPtr sockPtr, char* buffer, int len)
	{
		shared_ptr<Protocol::C2SDeleteDoYanTao>request = ProtobufParser::Decode<Protocol::C2SDeleteDoYanTao>("Protocol." + protoName[(int)ProtoID::C2SDeleteDoYanTao], buffer, len);
		vector<string>vec(request->name_size());
		DBManager::Instance().DeleteActivities("delete from doyantao where SBH='%s' and id='%s'", sockPtr->userID, vec);
	}
};

class C2SSearchDoWeiWenHandler :public HandleBase {
public:
	void HandleMessage(TcpSockPtr sockPtr, char* buffer, int len)
	{
		Protocol::S2CSearchDoWeiwen response;
		vector<Protocol::DoWeiWen>vec = DBManager::Instance().SearchDoWeiWen(sockPtr->userID);
		for (int i = 0; i < vec.size(); i++)
		{
			auto p = response.add_doweiwenlist();
			*p = vec[i];
		}
		char* data = ProtobufParser::Encode(ProtoID::S2CSearchDoWeiwen, response, len);
		sockPtr->Send(data, len);
	}
};

class C2SSearchWriteBookHandler :public HandleBase {
public:
	void HandleMessage(TcpSockPtr sockPtr, char* buffer, int len)
	{
		Protocol::S2CSearchWriteBook response;
		vector<Protocol::WriteBook>vec = DBManager::Instance().SearchWriteBook(sockPtr->userID);
		for (int i = 0; i < vec.size(); i++)
		{
			auto p = response.add_writebooklist();
			*p = vec[i];
		}
		char* data = ProtobufParser::Encode(ProtoID::S2CSearchWriteBook, response, len);
		sockPtr->Send(data, len);
	}
};