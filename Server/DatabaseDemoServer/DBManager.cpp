#include "DBManager.h"

DBManager& DBManager::Instance()
{
	static DBManager dbManager;
	return dbManager;
}
std::vector<Protocol::Writer> DBManager::SearchWriterList(const std::string& account)
{
	std::vector<Protocol::Writer>res;
	char sqlStr[] = "select * from writer where id='%s'";
	MYSQL_RES* mysqlRes = SearchResult(sqlStr, account);
	MYSQL_ROW row;
	while (row = mysql_fetch_row(mysqlRes))
	{
		Protocol::Writer w;
		w.set_bh(row[0]);
		w.set_name(row[1]);
		w.set_sex(row[2]);
		w.set_birth(row[3]);
		w.set_bmh(row[4]);
		res.push_back(w);
	}
	mysql_free_result(mysqlRes);
	return res;
}
DBManager::DBManager()
{
	mysql_init(&mysql);
}

MYSQL_RES* DBManager::SearchResult(char* sqlFormat, const std::string& account)
{
	char sql[1024];
	sprintf(sql, sqlFormat, account.c_str());
	if (mysql_query(&mysql, sql))
	{
		std::cout << "查询数据失败，sql语句为:" << sql << "  " << mysql_error(&mysql) << std::endl;
		return NULL;
	}
	return mysql_store_result(&mysql);
}

bool DBManager::CheckUser(std::string id, std::string pwd)
{
	MYSQL_RES* res;
	char sqlChar[] = "select * from account where id='%s' and pw='%s'";
	char sql[1024];
	sprintf(sql, sqlChar, id.c_str(), pwd.c_str());
	if (mysql_query(&mysql, sql))
	{
		std::cout << "数据库查询错误：" << mysql_errno(&mysql) << "   " << mysql_error(&mysql) << std::endl;
	}
	res = mysql_store_result(&mysql);
	/*bool isok = (res != NULL);
	mysql_free_result(res);
	return isok;*/

	MYSQL_ROW row;
	if (row = mysql_fetch_row(res))
		return true;
	return false;
	if (res != NULL)
	{
		while (row = mysql_fetch_row(res))
		{
			std::cout << row[0] << "  " << row[1] << std::endl;
		}
		return true;
	}

	return false;
}

bool DBManager::IsExistUser(const std::string& account)
{
	char sql[] = "select * from account where id='%s'";
	MYSQL_RES* res = SearchResult(sql, account);
	MYSQL_ROW row = mysql_fetch_row(res);
	mysql_free_result(res);
	return row;
}

void DBManager::AddUser(const std::string& account, const std::string& pwd)
{
	char sqlFmt[] = "insert into account set id='%s',pw='%s'";
	char sql[1024];
	sprintf(sql, sqlFmt, account.c_str(), pwd.c_str());
	if (mysql_query(&mysql, sql))
	{
		std::cout << "插入用户数据错误:" << mysql_error(&mysql) << std::endl;
	}
}

bool DBManager::AddWriters(const std::vector<Protocol::Writer>& writers, const std::string& userID)
{
	char sql[] = "insert into writer set BH='%s',name='%s',sex='%s',birth='%s',BMH='%s',id='%s'";
	char sqlStr[1024];
	for (auto& p : writers)
	{
		sprintf(sqlStr, sql, p.bh().c_str(), p.name().c_str(), p.sex().c_str(), p.birth().c_str(), p.bmh().c_str(), userID.c_str());
		if (mysql_query(&mysql, sqlStr))
		{
			std::cout << "插入作者信息异常,sql为:" << sql << "  " << mysql_error(&mysql) << std::endl;
			return false;
		}
	}
	return true;
}

void DBManager::UpdateWriters(const std::string& account, const std::vector<Protocol::UpdateWriter>& vec)
{
	char sql[] = "update writer set BH='%s',name='%s',sex='%s',birth='%s',BMH='%s' where BH='%s' and id='%s'";
	char sqlStr[1024];
	for (int i = 0; i < vec.size(); ++i)
	{
		sprintf(sqlStr, sql, vec[i].writer().bh().c_str(), vec[i].writer().name().c_str(), vec[i].writer().sex().c_str(),
			vec[i].writer().birth().c_str(), vec[i].writer().bmh().c_str(), vec[i].oldbh().c_str(), account.c_str());
		if (mysql_query(&mysql, sqlStr))
		{
			std::cout << "修改作者信息异常:" << mysql_error(&mysql) << std::endl;
			return;
		}
	}
}

void DBManager::AddBooks(const std::string& account, const std::vector<Protocol::Book>& vec)
{
	char sqlStr[] = "insert into book set BH='%s',name='%s',time='%s',price='%f',id='%s'";
	char sql[1024];
	for (int i = 0; i < vec.size(); ++i)
	{
		sprintf(sql, sqlStr, vec[i].bh().c_str(), vec[i].name().c_str(), vec[i].time().c_str(), vec[i].price(), account.c_str());
		if (mysql_query(&mysql, sql))
		{
			std::cout << "插入书籍失败:" << mysql_error(&mysql) << std::endl;
			return;
		}
	}
}

void DBManager::UpdateBooks(const std::string& account, const std::vector<Protocol::UpdateBook>& vec)
{
	char sqlStr[] = "update book set BH='%s',name='%s',time='%s',price='%f' where id='%s' and BH='%s'";
	char sql[1024];
	for (int i = 0; i < vec.size(); ++i)
	{
		Protocol::Book book = vec[i].book();
		sprintf(sql, sqlStr, book.bh().c_str(), book.name().c_str(), book.time().c_str(), book.price(), account.c_str(), vec[i].oldbh().c_str());
		if (mysql_query(&mysql, sql))
		{
			std::cout << "修改书籍数据错误:" << mysql_error(&mysql) << std::endl;
			return;
		}
	}
}

/// <summary>
/// 主键(一个字符串)+用户ID(一个字符串)删除数据
/// sql语句必须是主键在前，用户ID在后
/// </summary>
void DBManager::DeleteDataByString(const char* sqlFormat, const std::string& account, const std::vector<std::string>& vec)
{
	char sql[1024];
	for (int i = 0; i < vec.size(); ++i)
	{
		sprintf(sql, sqlFormat, vec[i].c_str(), account.c_str());
		if (mysql_query(&mysql, sql))
		{
			std::cout << "删除数据错误,sql语句为：" << sql << " " << mysql_error(&mysql) << std::endl;
			return;
		}
	}
}

void DBManager::AddYanTao(const std::string& account, const std::vector<Protocol::YanTao>& vec)
{
	char sqlStr[] = "insert into yantao set name='%s',Content='%s',time='%s',address='%s', id = '%s'";
	char sql[1024];
	for (int i = 0; i < vec.size(); ++i)
	{
		sprintf(sql, sqlStr, vec[i].name().c_str(), vec[i].content().c_str(), vec[i].time().c_str(), vec[i].address().c_str(), account.c_str());
		if (mysql_query(&mysql, sql))
		{
			std::cout << "插入研讨数据失败：" << mysql_error(&mysql) << std::endl;
			return;
		}
	}
}

void DBManager::UpdateYanTao(const std::string& account, const std::vector<Protocol::UpdateYanTao>& vec)
{
	char sqlStr[] = "update yantao set name='%s',Content='%s',time='%s',address='%s' where id='%s' and name='%s'";
	char sql[1024];
	for (int i = 0; i < vec.size(); ++i)
	{
		Protocol::UpdateYanTao upY = vec[i];
		sprintf(sql, sqlStr, upY.yantao().name().c_str(), upY.yantao().content().c_str(),
			upY.yantao().time().c_str(), upY.yantao().address().c_str(), account.c_str(), upY.oldname().c_str());
		if (mysql_query(&mysql, sql))
		{
			std::cout << "更新研讨数据异常:" << mysql_error(&mysql) << std::endl;
			return;
		}
	}
}

void DBManager::AddDep(const std::string& account, const std::vector<Protocol::Department>& vec)
{
	char sqlStr[] = "insert into department set BMH='%s',name='%s',id='%s'";
	char sql[1024];
	for (int i = 0; i < vec.size(); ++i)
	{
		sprintf(sql, sqlStr, vec[i].bmh().c_str(), vec[i].name().c_str(), account.c_str());
		if (mysql_query(&mysql, sql))
		{
			std::cout << "插入部门数据失败：" << mysql_error(&mysql) << std::endl;
			return;
		}
	}
}

void DBManager::UpdateDep(const std::string& account, const std::vector<Protocol::UpdateDepartment>& vec)
{
	char sqlStr[] = "update department set BMH='%s',name='%s' where BMH='%s' and id='%s'";
	char sql[1024];
	for (int i = 0; i < vec.size(); ++i)
	{
		sprintf(sql, sqlStr, vec[i].department().bmh().c_str(), vec[i].department().name().c_str(), vec[i].oldbh().c_str(), account.c_str());
		if (mysql_query(&mysql, sql))
		{
			std::cout << "修改部门数据失败:" << mysql_error(&mysql) << std::endl;
			return;
		}
	}
}

void DBManager::AddActivities(const char* sqlFormat, const std::string& account, const std::vector<std::vector<std::string>>& vec)
{
	char sql[1024];
	for (int i = 0; i < vec.size(); ++i)
	{
		sprintf(sql, sqlFormat, vec[i][0].c_str(), vec[i][1].c_str(), account.c_str());
		if (mysql_query(&mysql, sql))
		{
			std::cout << "插入数据错误：" << mysql_error(&mysql) << std::endl;
			return;
		}
	}

}

void DBManager::UpdateActivities(const char* sqlFormat, const std::string& account, const std::vector<std::vector<std::string>>& vec, const std::vector<std::string>& oldPK)
{
	char sql[1024];
	for (int i = 0; i < vec.size(); ++i)
	{
		sprintf(sql, sqlFormat, vec[i][0].c_str(), vec[i][1].c_str(), oldPK[i].c_str(), account.c_str());
		if (mysql_query(&mysql, sql))
		{
			std::cout << "修改活动数据异常:" << mysql_error(&mysql) << std::endl;
			return;
		}
	}
}

void DBManager::DeleteActivities(const char* sqlFmt, const std::string& account, const std::vector<std::string>& vec)
{
	char sql[1024];
	for (int i = 0; i < vec.size(); ++i)
	{
		sprintf(sql, sqlFmt, vec[i].c_str(), account.c_str());
		if (mysql_query(&mysql, sql))
		{
			std::cout << "删除活动数据错误:" << mysql_error(&mysql) << std::endl;
			return;
		}
	}
}

std::vector<Protocol::Department> DBManager::SearchDep(const std::string& account)
{
	char sqlFormat[] = "select * from department where id='%s'";
	MYSQL_RES* res = SearchResult(sqlFormat, account);
	MYSQL_ROW row;
	std::vector<Protocol::Department>vec;
	while (row = mysql_fetch_row(res))
	{
		Protocol::Department dep;
		dep.set_bmh(row[0]);
		dep.set_name(row[1]);
		vec.push_back(dep);
	}
	mysql_free_result(res);
	return vec;
}

void DBManager::AddWeiWen(const std::string& account, const std::vector<Protocol::WeiWen>& vec)
{
	char sqlFmt[] = "insert into weiwen set name='%s',time='%s',address='%s',id='%s'";
	char sql[1204];
	for (int i = 0; i < vec.size(); ++i)
	{
		sprintf(sql, sqlFmt, vec[i].name().c_str(), vec[i].time().c_str(), vec[i].address().c_str(), account.c_str());
		if (mysql_query(&mysql, sql))
		{
			std::cout << "插入慰问数据异常：" << mysql_error(&mysql) << std::endl;
			return;
		}
	}
}

void DBManager::UpdateWeiWen(const std::string& account, const std::vector<Protocol::UpdateWeiWen>& vec)
{
	char sqlFmt[] = "update weiwen set name='%s',time='%s',address='%s' where name='%s' and id='%s'";
	char sql[1024];
	for (int i = 0; i < vec.size(); ++i)
	{
		Protocol::UpdateWeiWen uww;
		uww = vec[i];
		sprintf(sql, sqlFmt, uww.weiwen().name().c_str(), uww.weiwen().time().c_str(),
			uww.weiwen().address().c_str(), uww.oldname().c_str(), account.c_str());
		if (mysql_query(&mysql, sql))
		{
			std::cout << "修改慰问数据异常:" << mysql_error(&mysql) << std::endl;
			return;
		}
	}
}

std::vector<Protocol::DoYanTao> DBManager::SearchDoYanTao(const std::string& account)
{
	std::vector<Protocol::DoYanTao>vec;
	char sql[] = "select * from doyantao where id='%s'";
	MYSQL_RES* res = SearchResult(sql, account);
	MYSQL_ROW row;
	while (row = mysql_fetch_row(res))
	{
		Protocol::DoYanTao dy;
		dy.set_name(row[0]);
		dy.set_sbh(row[1]);
		vec.push_back(dy);
	}
	mysql_free_result(res);
	return vec;

}

std::vector<Protocol::DoWeiWen> DBManager::SearchDoWeiWen(const std::string& account)
{
	std::vector<Protocol::DoWeiWen>vec;
	char sql[] = "select * from doweiwen where id='%s'";
	MYSQL_RES* res = SearchResult(sql, account);
	MYSQL_ROW row;
	while (row = mysql_fetch_row(res))
	{
		Protocol::DoWeiWen dy;
		dy.set_sbh(row[1]);
		dy.set_name(row[0]);
		vec.push_back(dy);
	}
	mysql_free_result(res);
	return vec;
}

std::vector<Protocol::WriteBook> DBManager::SearchWriteBook(const std::string& account)
{
	std::vector<Protocol::WriteBook>vec;
	char sql[] = "select * from writebook where id='%s'";
	MYSQL_RES* res = SearchResult(sql, account);
	MYSQL_ROW row;
	while (row = mysql_fetch_row(res))
	{
		Protocol::WriteBook dy;
		dy.set_bbh(row[0]);
		dy.set_sbh(row[1]);
		vec.push_back(dy);
	}
	mysql_free_result(res);
	return vec;
}

std::vector<Protocol::Book> DBManager::SearchBooks(const std::string& account)
{
	char sqlStr[] = "select * from book where id='%s'";

	MYSQL_RES* result = SearchResult(sqlStr, account);
	MYSQL_ROW row;
	std::vector<Protocol::Book >vec;
	while (row = mysql_fetch_row(result))
	{
		Protocol::Book book;
		book.set_bh(row[0]);
		book.set_name(row[1]);
		book.set_time(row[2]);
		book.set_price(atof(row[3]));
		vec.push_back(book);
	}
	mysql_free_result(result);
	return vec;
}

bool DBManager::AddWriteBook(const std::string& account, const std::vector<Protocol::WriteBook>& vec)
{
	char sqlStr[] = "insert into writebook set BBH='%s',SBH='%s',id='%s'";
	char sql[1024];
	for (int i = 0; i < vec.size(); ++i)
	{
		sprintf(sql, sqlStr, vec[i].bbh().c_str(), vec[i].sbh().c_str(), account.c_str());
		if (mysql_query(&mysql, sql))
		{
			std::cout << "插入出书记录异常:" << mysql_error(&mysql) << std::endl;
			return false;
		}
	}
	return true;
}

std::vector<Protocol::WeiWen> DBManager::SearchWeiWen(const std::string& account)
{
	std::vector<Protocol::WeiWen>vec;
	char sql[] = "select * from weiwen where id='%s'";

	MYSQL_RES* res = SearchResult(sql, account);
	MYSQL_ROW row;
	while (row = mysql_fetch_row(res))
	{
		Protocol::WeiWen weiwen;
		weiwen.set_name(row[0]);
		weiwen.set_time(row[1]);
		weiwen.set_address(row[2]);
		vec.push_back(weiwen);
	}
	mysql_free_result(res);
	return vec;
}

void DBManager::AddDoWeiWen(const std::string& account, const std::vector<Protocol::DoWeiWen> vec)
{
	char sqlStr[] = "insert into doweiwen set name='%s',SBH='%s',id='%s'";
	char sql[1024];
	for (int i = 0; i < vec.size(); ++i)
	{
		sprintf(sql, sqlStr, vec[i].name().c_str(), vec[i].sbh().c_str(), account.c_str());
		if (mysql_query(&mysql, sql))
		{
			std::cout << "插入慰问活动记录失败:" << mysql_error(&mysql) << std::endl;
			return;
		}
	}
}

std::vector<Protocol::YanTao> DBManager::SearchYanTao(const std::string& account)
{
	std::vector<Protocol::YanTao>vec;
	char sql[] = "select * from yantao where id='%s'";

	MYSQL_RES* res = SearchResult(sql, account);
	MYSQL_ROW row;
	while (row = mysql_fetch_row(res))
	{
		Protocol::YanTao yantao;
		yantao.set_name(row[0]);
		yantao.set_content(row[1]);
		yantao.set_time(row[2]);
		yantao.set_address(row[3]);
		vec.push_back(yantao);
	}
	return vec;
}

void DBManager::AddDoYanTao(std::string& account, std::vector<Protocol::DoYanTao>& vec)
{
	char sqlStr[] = "insert into doyantao set SBH='%s',name='%s',id='%s'";
	char sql[1024];
	for (int i = 0; i < vec.size(); ++i)
	{
		sprintf(sql, sqlStr, vec[i].sbh().c_str(), vec[i].name().c_str(), account.c_str());
		if (mysql_query(&mysql, sql))
		{
			std::cout << "插入研讨活动时失败：" << mysql_error(&mysql) << std::endl;
			return;
		}
	}
}

void DBManager::Connect()
{
	if (!mysql_real_connect(&mysql, host, user, password, dbName, 3306, 0, 0))
	{
		std::cout << "数据库 " << dbName << " 连接失败\n";
	}
	else
	{
		std::cout << "数据库 " << dbName << " 连接成功\n";
	}
}
