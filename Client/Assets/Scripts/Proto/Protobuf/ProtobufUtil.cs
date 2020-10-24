using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Google.Protobuf;
using System.Text;
using System;
using System.IO;
using Protocol;

public enum ProtoID
{
    C2SLogin = 0,
    S2CLogin,
    C2SRegister,
    S2CRegister,
    C2SWriter,
    S2CWriter,
    C2SAddWriters,
    S2CAddWriters,
    C2SBook,
    S2CBook,
    C2SAddBooks,
    C2SDeleteBooks,
    UpdateBook,
    C2SUpdateBooks,
    C2SYanTao,
    S2CYanTao,
    C2SWeiWen,
    S2CWeiWen,
    C2SWriteBook,
    S2CWriteBook,
    C2SDoYanTao,
    S2CDoYanTao,
    C2SDoWeiWen,
    S2cDoWeiWen,
    C2SAddWriteBook,
    C2SAddDoWeiWen,
    C2SAddDoYanTao,
    C2SDeleteWriters,
    C2SUpdateWriters,
    C2SAddYanTao,
    UpdateYanTao,
    C2SUpdateYanTao,
    C2SDeleteYanTao,
    C2SDepartment,
    S2CDepartment,
    C2SAddDepartment,
    UpdateDepartment,
    C2SUpdateDepartment,
    C2SDeleteDepartment,
    C2SAddWeiWen,
    UpdateWeiWen,
    C2SUpdateWeiWen,
    C2SDeleteWeiWen,
    C2SSearchDoYanTao,
    S2CSearchDoYanTao,
    UpdateDoYanTao,
    C2SUpdateDoYanTao,
    C2SDeleteDoYanTao,
    C2SSearchWriteBook,
    S2CSearchWriteBook,
    C2SSearchDoWeiwen,
    S2CSearchDoWeiwen
}

public static class ProtobufParser
{
    private static Dictionary<IMessage, string> protoName = new Dictionary<IMessage, string>();

    /// <summary>
    /// 编码协议名称
    /// 前两个字节是名字的长度
    /// </summary>
    public static byte[] EncodeName(IMessage message)
    {
        string name = "";
        if (protoName.TryGetValue(message, out name) == false)
        {
            name = message.GetType().ToString();
            protoName[message] = name;
        }
        byte[] head = Encoding.UTF8.GetBytes(name);
        Int16 len = (Int16)head.Length;
        byte[] bytes = new byte[len + 2];
        bytes[0] = (byte)(len % 256);
        bytes[1] = (byte)(len / 256);
        Array.Copy(head, 0, bytes, 2, len);
        return bytes;
    }

    /// <summary>
    /// 编码协议内容
    /// </summary>
    public static byte[] EncodeBody(IMessage message)
    {
        using (var memory = new MemoryStream())
        {
            message.WriteTo(memory);
            return memory.ToArray();
        }
    }

    /// <summary>
    /// 编码完整的协议
    /// 最终格式为:两字节总体长度+两字节协议名长度+协议名+协议内容
    /// </summary>
    public static byte[] EncodeByName(IMessage message)
    {
        byte[] head = EncodeName(message);
        Int16 headLne = (Int16)head.Length;
        byte[] body = EncodeBody(message);
        Int16 bodyLen = (Int16)body.Length;
        Int16 len = (Int16)(headLne + bodyLen);
        byte[] bytes = new byte[len + 2];
        bytes[0] = (byte)(len % 256);
        bytes[1] = (byte)(len / 256);
        Array.Copy(head, 0, bytes, 2, headLne);
        Array.Copy(body, 0, bytes, headLne + 2, bodyLen);
        return bytes;
    }

    /// <summary>
    /// 消息长度(2字节)+协议编号(2字节)+协议体
    /// </summary>
    /// <param name="protoID"></param>
    /// <param name="message"></param>
    /// <returns></returns>
    public static byte[] Encode(ProtoID protoID, IMessage message)
    {
        byte[] body = EncodeBody(message);
        int len = body.Length + 2;
        byte[] datas = new byte[len + 2];
        datas[0] = (byte)(len % 256);
        datas[1] = (byte)(len / 256);
        datas[2] = (byte)((int)protoID % 256);
        datas[3] = (byte)((int)protoID / 256);
        Array.Copy(body, 0, datas, 4, body.Length);
        return datas;
    }

    public static IMessage Decode(int protoID, byte[] bytes, int offset, int len)
    {
        //T message = default(T);
        //try
        //{
        //    message = new T();
        //    message.MergeFrom(bytes, offset, len);
        //    return message;
        //}
        //catch (Exception ex)
        //{
        //    Debug.LogError("协议解析失败" + ex.ToString());
        //}
        //return message;
        string proto = System.Enum.GetName(typeof(ProtoID), protoID);
        Type t = Type.GetType("Protocol." + proto);
        if (t == null)
        {
            Debug.LogError("解析协议失败，枚举反射错误");
            return null;
        }
        IMessage message = (IMessage)Activator.CreateInstance(t);
        message.MergeFrom(bytes, offset, len);
        return message;
    }

    public static IMessage Decode(byte[] bytes, int offset, int len)
    {
        Int16 headLen = BitConverter.ToInt16(bytes, offset);
        string name = Encoding.UTF8.GetString(bytes, offset + 2, headLen);
        Debug.Log("协议名:" + name);
        IMessage message = null;
        try
        {
            Type t = Type.GetType(name);
            message = (IMessage)Activator.CreateInstance(t);
            Int16 bodyLen = (Int16)(len - 2 - headLen);
            message.MergeFrom(bytes, offset + 2 + headLen, bodyLen);
        }
        catch (Exception e)
        {
            Debug.LogError("协议解析错误,协议名为:" + name + "   " + e.ToString());
            return message;
        }

        //Debug.Log("解析到:"+((S2CLogin)message).Account);
        return message;
    }

    public static T Decode<T>(byte[] bytes, int offset, int len) where T : IMessage, new()
    {
        T message = default(T);
        try
        {
            message = new T();
            message.MergeFrom(bytes, offset, len);
            return message;
        }
        catch (Exception ex)
        {
            Debug.LogError("协议解析失败：" + ex.ToString());
        }
        return message;
    }

}
