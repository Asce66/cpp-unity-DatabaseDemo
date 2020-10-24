using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System;
using System.Linq;

public class MsgBase
{
    //协议名称
    public string protoName;

    /// <summary>
    /// 编码协议
    /// </summary>
    public static byte[]Encode(MsgBase msg)
    {
        string str = JsonUtility.ToJson(msg);
        byte[] data = Encoding.UTF8.GetBytes(str);
        return data;
    }

    /// <summary>
    /// 编码协议名
    /// </summary>
    public static byte[]EncodeName(MsgBase msgBase)
    {
        byte[] nameByte = Encoding.UTF8.GetBytes(msgBase.protoName);
        Int16 len = (Int16)nameByte.Length;

        //小端模式存放协议名称长度
        byte[] bytes = new byte[2 + len];
        bytes[0] = (byte)(len % 256);
        bytes[1] = (byte)(len / 256);
        //Debug.Log("长度："+Encoding.UTF8.GetString())

        Array.Copy(nameByte, 0, bytes, 2, len);
       // bytes = bytes.Concat(nameByte).ToArray();
        string str = Encoding.UTF8.GetString(bytes);
        return bytes;
    }

    /// <summary>
    /// 解码协议
    /// </summary>
    public static MsgBase Decode(string protoname, byte[]data,int offset,int count)
    {
        string str = Encoding.UTF8.GetString(data,offset,count);
        MsgBase msg = (MsgBase)JsonUtility.FromJson(str, Type.GetType(protoname));
        return msg;
    }

    /// <summary>
    /// 解析协议名称
    /// </summary>
    public static string DecodeName(byte[]bytes,int offset,out int count)
    {
        count = 0;
        //协议名称长度已经占用2个字节长度，数据长度所以不能比2小
        if (offset+2> bytes.Length) return "";
        //读取长度
        Int16 len = (Int16)((bytes[offset+1]<<8)|bytes[offset]);

        //判断长度是否可以解析出来协议名称
        if (offset + 2 + len > bytes.Length)
            return "";
        //解析
        count = len + 2;
        string name = Encoding.UTF8.GetString(bytes, offset + 2, len);
        return name;
    }

}
