#pragma once
#include<mysql.h>
#pragma comment(lib, "libmysql.lib")

#include<string>
#include<iostream>
#include"MsgUser.pb.h"
#include"MsgData.pb.h"

class DBManager
{
private:
	//数据库配置信息
	const char* host = "127.0.0.1";
	const char* user = "root";
	const char* password = "31415926ASL";
	const char* dbName = "publishwritermnr";

	MYSQL mysql;
	DBManager();
	MYSQL_RES* SearchResult(char* sqlFormat, const std::string& account);
public:
	bool CheckUser(std::string id, std::string pwd);
	bool IsExistUser(const std::string& account);
	void AddUser(const std::string& account, const std::string& pwd);
	static DBManager& Instance();
	std::vector<Protocol::Writer>SearchWriterList(const std::string& account);
	DBManager operator=(const DBManager& db) = delete;
	DBManager(const DBManager& db) = delete;
	bool AddWriters(const std::vector<Protocol::Writer>& writers, const std::string& userID);
	std::vector<Protocol::Book>SearchBooks(const std::string& account);
	bool AddWriteBook(const std::string& account, const std::vector<Protocol::WriteBook>& vec);
	std::vector<Protocol::WeiWen>SearchWeiWen(const std::string& account);
	void AddDoWeiWen(const std::string& account, const std::vector<Protocol::DoWeiWen>vec);
	std::vector<Protocol::YanTao>SearchYanTao(const std::string& account);
	void AddDoYanTao(std::string& account, std::vector<Protocol::DoYanTao>& vec);
	void UpdateWriters(const std::string& account, const std::vector<Protocol::UpdateWriter>& vec);
	void AddBooks(const std::string& account, const std::vector<Protocol::Book>& vec);
	void UpdateBooks(const std::string& account, const std::vector<Protocol::UpdateBook>& vec);
	void DeleteDataByString(const char* sqlFormat, const std::string& account, const std::vector<std::string>& vec);
	void AddYanTao(const std::string& account, const std::vector<Protocol::YanTao>& vec);
	void UpdateYanTao(const std::string& account, const std::vector<Protocol::UpdateYanTao>& vec);
	void AddDep(const std::string& account, const std::vector<Protocol::Department>& vec);
	void UpdateDep(const std::string& account, const std::vector<Protocol::UpdateDepartment>& vec);
	void AddActivities(const char* sqlFormat, const std::string& account, const std::vector<std::vector<std::string>>& vec);
	void UpdateActivities(const char* sqlFormat, const std::string& account, const std::vector<std::vector<std::string>>& vec, const std::vector<std::string>& oldPK);
	void DeleteActivities(const char* sqlFmt, const std::string& account, const std::vector<std::string>& vec);
	std::vector<Protocol::Department>SearchDep(const std::string& account);
	void AddWeiWen(const std::string& account, const std::vector<Protocol::WeiWen>& vec);
	void UpdateWeiWen(const std::string& account, const std::vector<Protocol::UpdateWeiWen>& vec);
	std::vector<Protocol::DoYanTao>SearchDoYanTao(const std::string& account);
	std::vector<Protocol::DoWeiWen>SearchDoWeiWen(const std::string& account);
	std::vector<Protocol::WriteBook>SearchWriteBook(const std::string& account);
	void Connect();

	~DBManager()
	{
		mysql_close(&mysql);
	}
};
