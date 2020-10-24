#include "CharArray.h"

void CharArray::CheckAndMove()
{
	if (Length() < 8)
		MoveData();
}

void CharArray::MoveData()
{
	memcpy(data, data + readIndex, Length());
	writeIndex = Length();
	readIndex = 0;
}

CharArray::CharArray(int size)	
{
	data = new  char[size];
	capacity = size;
	readIndex = writeIndex = 0;
}

CharArray::CharArray(char* str, int len)
{
	data = new char[len];
	memcpy(data, str, len);
	capacity = len;
	readIndex = 0;
	writeIndex = len;
	delete str;
}

std::string CharArray::GetString(int count)
{
	std::string str(data + readIndex, data + readIndex + count);
	//readIndex += count;
	return str;
}

int CharArray::GetInt()
{
	if (Length()<2)
	{
		std::cout << "剩余数据长度已经不足以取出一个int值\n";
		return -2;
	}
	int res = (data[readIndex + 1] << 8) |data[readIndex];
	return res;
}

CharArray::~CharArray()
{
	if (data != nullptr)
	{
		std::cout << "delete CharArray\n";
		delete data;
	}
		
}
