#pragma once
#include<iostream>
#include<memory>
class CharArray
{
public:
	static const int DEFAULT_SIZE = 1024;
	char* data = nullptr;
	/// <summary>
	/// �����ݵ���ʼ�±�
	/// </summary>
	int readIndex;
	/// <summary>
	/// д���ݵ���ʼ�±�
	/// </summary>
	int writeIndex;

	/// <summary>
	/// �������������С
	/// </summary>
	int capacity;

	/// <summary>
	/// ʣ��ɴ�����ݴ�С
	/// </summary>
	int Remain()
	{
		return capacity - writeIndex;
	}

	/// <summary>
	/// �ִ����ݳ���
	/// </summary>
	int Length()
	{
		return writeIndex - readIndex;
	}

	void CheckAndMove();

	void MoveData();

	/// <summary>
	/// ���ڽ������ݵĻ���
	/// </summary>
	/// <param name="size"></param>
	CharArray(int size = DEFAULT_SIZE);

	CharArray(char* str, int len);

	std::string GetString(int count);

	int GetInt();

	~CharArray();
};

