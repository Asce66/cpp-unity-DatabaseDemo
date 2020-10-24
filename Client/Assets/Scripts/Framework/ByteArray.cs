using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ByteArray
{
    //默认大小
    const int DEFAULT_SIZE = 1024;
    //缓冲区
    public byte[] data;
    //读/写数据位置
    public int readIndex;
    public int writeIndex;
    //初始化长度（后期扩容用）
    public int initSize = 0;
    //容量
    public int capacity;
    //剩余空间长度
    public int remain
    {
        get
        {
            return capacity - writeIndex;
        }
    }
    //数据长度
    public int length
    {
        get
        {
            return writeIndex - readIndex; 
        }
    }

    /// <summary>
    /// 用于接收数据的缓存
    /// </summary>
    /// <param name="size"></param>
    public ByteArray(int size = DEFAULT_SIZE)
    {
        data = new byte[size];       
        capacity = size;
        initSize = size;
        readIndex = 0;
        writeIndex = 0;
    }

    /// <summary>
    /// 用于发送数据的缓存
    /// </summary>
    /// <param name="bytes"></param>
    public ByteArray(byte[] bytes)
    {
        data = bytes;
        initSize = bytes.Length;
        capacity = bytes.Length;
        readIndex = 0;
        writeIndex = bytes.Length;
    }

    /// <summary>
    /// 重设缓存大小
    /// </summary>
    /// <param name="size"></param>
    /// <returns></returns>
    public void Resize(int size)
    {
        if (size < length || size < initSize) return;
        int n = 1;
        while (n < size)
            n *= 2;
        capacity = n;
        byte[] newData = new byte[capacity];
        Array.Copy(data, readIndex, newData, 0, length);
        data = newData;
        writeIndex = length;
        readIndex = 0;
    }

    /// <summary>
    /// 当缓存内的数组比较少时，就把数据向前移动，从0下标开始排列
    /// </summary>
    public void CheckAndMoveData()
    {
        if (length < 8)
            MoveData();
    }

    public void MoveData()
    {
        Array.Copy(data, readIndex, data, 0, length);
        writeIndex = length;
        readIndex = 0;
    }

    public byte[]ReadData(int startInd,int count)
    {
        byte[] bytes = new byte[count];
        Array.Copy(data, startInd, bytes, 0, count);
        readIndex += count;
        CheckAndMoveData();
        return bytes;
    }

}