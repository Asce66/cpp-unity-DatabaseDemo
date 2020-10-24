#pragma once
#include<iostream>
#include<memory>
class CharArray
{
public:
	static const int DEFAULT_SIZE = 1024;
	char* data = nullptr;
	/// <summary>
	/// 读数据的起始下标
	/// </summary>
	int readIndex;
	/// <summary>
	/// 写数据的起始下标
	/// </summary>
	int writeIndex;

	/// <summary>
	/// 整个缓存数组大小
	/// </summary>
	int capacity;

	/// <summary>
	/// 剩余可存放数据大小
	/// </summary>
	int Remain()
	{
		return capacity - writeIndex;
	}

	/// <summary>
	/// 现存数据长度
	/// </summary>
	int Length()
	{
		return writeIndex - readIndex;
	}

	void CheckAndMove();

	void MoveData();

	/// <summary>
	/// 用于接收数据的缓存
	/// </summary>
	/// <param name="size"></param>
	CharArray(int size = DEFAULT_SIZE);

	CharArray(char* str, int len);

	std::string GetString(int count);

	int GetInt();

	~CharArray();
};

