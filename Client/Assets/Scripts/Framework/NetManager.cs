using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;
using System.Net;
using System;
using System.Text;
using Protocol;
using Google.Protobuf;

public static class NetManager
{
    //套接字
    static Socket socket;
    //缓冲区
    static ByteArray readBuff;
    //写入队列
    static Queue<ByteArray> writeQueue;

    //网络事件监听委托
    public delegate void EventListener(string msg);
    //所有事件监听存放字典
    private static Dictionary<NetEvent, EventListener> eventListeners = new Dictionary<NetEvent, EventListener>();

    public delegate void MessageHandler(IMessage message);
    private static Dictionary<int, MessageHandler> messageHandler = new Dictionary<int, MessageHandler>();

    ////消息事件监听委托
    //public delegate void MsgListener(MsgBase msgBase);
    ////事件监听字典
    //private static Dictionary<string, MsgListener> msgListener = new Dictionary<string, MsgListener>();

    ///<summary>
    ///是否正在连接中
    ///防止程序正在连接但还没连接上服务器时
    ///用户又点击一次连接导致发送了多次连接请求
    ///</summary>
    private static bool isConnecting = false;

    //消息队列中待处理的消息(封装了一下协议编号，方便处理协议时找到对应的Handler)
    private class MessageToDo
    {
        public IMessage message;
        public int protoID;
    }

    //消息队列
    private static Queue<MessageToDo> msgQueue = new Queue<MessageToDo>();
    private static int msgCount = 0;//消息队列大小（避免线程问题 ）
    private readonly static int MAX_MESSAGE_FIRE = 10;

    ///<summary>
    ///是否正在关闭连接
    ///发送消息使用的消息队列，如果客户端选择关闭连接
    ///需要等待把消息队列的所有消息发送完成才真正断开连接
    ///</summary>
    private static bool isClosing = false;

    /// <summary>
    /// 心跳协议
    /// 检测客户端是否还在与服务器连接
    /// TCP有自定义的心跳协议，但是时长是2小时左右，不适用
    /// 客户端每隔一定时间就向服务器发送Ping协议
    /// 服务器收到就回复Pong协议
    /// 如果客户端很长时间没有受到Pong协议
    /// 则连接已经断开，释放socket资源
    /// </summary>
    public static bool isUsingPing = true;//是否启用心跳协议
    private static float pingInterval = 10;//每一次心跳协议的间隔
    private static float lastPingTime;//上次发送Ping协议的时间
    private static float lastPongTime;//上次接受到Pong协议的时间

    public static void Update()
    {
        UpdateMsgQueue();
        //UpdatePing();
    }


    private static void UpdateMsgQueue()
    {
        if (msgCount <= 0) return;
        for (int i = 0; i < MAX_MESSAGE_FIRE; i++)
        {
            MessageToDo messageToDo = null;
            lock (msgQueue)
            {
                if (msgQueue.Count > 0)
                    messageToDo = msgQueue.Dequeue();
            }
            if (messageToDo != null)
            {
                if (messageHandler.ContainsKey(messageToDo.protoID))
                    messageHandler[messageToDo.protoID](messageToDo.message);
                else
                    Debug.LogWarning("没有Handler:" + messageToDo.protoID);
            }
            else
                break;
        }
    }

    private static void UpdatePing()
    {
        if (isUsingPing == false) return;
        //检测是否发送Ping协议
        if (Time.time - lastPingTime > pingInterval)
        {
            MsgPing msgPing = new MsgPing();
            Send(msgPing);
            lastPingTime = Time.time;
        }
        //检测Pong协议接收时间，即判断是否掉线
        if (Time.time - lastPongTime > pingInterval * 4)
            Close();
    }

    /// <summary>
    /// 连接服务器
    /// </summary>
    public static void Connect(string ip, int port)
    {
        //判断socket是否已经连接
        if (socket != null && socket.Connected)
        {
            Debug.Log("程序已经连接，不需要再次请求连接");
            return;
        }

        //判断是否正在连接
        if (isConnecting == true)
        {
            Debug.Log("请求连接失败！程序正在连接中");
            return;
        }

        InitState();
        isConnecting = true;

        ///<summary>
        ///关闭Delay算法
        ///Delay算法即为当程序在多次向服务端发送很短的信息片段时
        ///会把多个短信息合并成一个大的包再发送出去
        ///因此会影响发送效率
        ///</summary>
        socket.NoDelay = true;
        IPEndPoint iPEndPoint = new IPEndPoint(IPAddress.Parse(ip), port);
        socket.Connect(iPEndPoint);
        FireEvent(NetEvent.ConnectSucc, "连接服务器成功");
        isConnecting = false;
        socket.BeginReceive(readBuff.data, readBuff.writeIndex, readBuff.remain, 0, ReceiveCallback, socket);

        //socket.BeginConnect(iPEndPoint, ConnectCallback, socket);
    }

    private static void ConnectCallback(IAsyncResult asyncResult)
    {
        try
        {
            Socket socket = (Socket)asyncResult.AsyncState;
            socket.EndConnect(asyncResult);
            FireEvent(NetEvent.ConnectSucc, "连接服务器成功");
            isConnecting = false;
            socket.BeginReceive(readBuff.data, readBuff.writeIndex, readBuff.remain, 0, ReceiveCallback, socket);
        }
        catch (Exception ex)
        {
            Debug.Log("连接服务器失败：" + ex.ToString());
            FireEvent(NetEvent.ConnectFail, ex.ToString());
            isConnecting = false;
        }
    }

    private static void ReceiveCallback(IAsyncResult asyncResult)
    {
        try
        {
            Socket socket = (Socket)asyncResult.AsyncState;
            int len = socket.EndReceive(asyncResult);
            if (len <= 0)
            {
                Close();
                return;
            }
            readBuff.writeIndex += len;

            OnReceiveData();
            if (readBuff.remain < 8)
            {
                readBuff.MoveData();
                readBuff.Resize(readBuff.length * 2);
            }
            socket.BeginReceive(readBuff.data, readBuff.writeIndex, readBuff.remain, 0, ReceiveCallback, socket);
        }
        catch (Exception e)
        {
            Debug.LogError("解析数据错误:" + e.ToString());
        }
    }

    private static void OnReceiveData()
    {
        while (readBuff.length > 2)
        {
            //获得协议整体长度
            byte[] bytes = readBuff.data;
            int bodyLen = (bytes[readBuff.readIndex + 1] << 8) | bytes[readBuff.readIndex];
            if (readBuff.length - 2 < bodyLen)
                break;
            readBuff.readIndex += 2;
            //解析协议编号
            int mProtoID = -1;
            mProtoID = BitConverter.ToInt16(bytes, readBuff.readIndex);
            readBuff.readIndex += 2;
            //解析协议
            IMessage mMessage = ProtobufParser.Decode(mProtoID, readBuff.data, readBuff.readIndex, bodyLen - 2);
            MessageToDo messageToDo = new MessageToDo() { message = mMessage, protoID = mProtoID };

            lock (writeQueue)
            {
                msgQueue.Enqueue(messageToDo);
            }
            msgCount++;
            readBuff.readIndex += bodyLen - 2;
            readBuff.CheckAndMoveData();
        }
    }


    public static void Send(byte[] data)
    {
        if (socket == null || socket.Connected == false) return;
        if (isClosing || isConnecting) return;
        ByteArray byteArray = new ByteArray(data);
        int count = 0;
        lock (writeQueue)
        {
            writeQueue.Enqueue(byteArray);
            count = writeQueue.Count;
        }
        if (count == 1)
        {
            socket.BeginSend(readBuff.data, readBuff.readIndex, readBuff.length, 0, SendCallback, socket);
        }
    }

    /// <summary>
    /// 用于Json的数据发送
    /// </summary>
    /// <param name="msgBase"></param>
    public static void Send(MsgBase msgBase)
    {
        if (socket == null || socket.Connected == false) return;
        if (isClosing || isConnecting) return;

        //数据编码
        // 9  MsgMove{ "protoName":"MsgMove","x":1.0,"y":0.0,"z":13.0}
        byte[] nameByte = MsgBase.EncodeName(msgBase);
        Debug.Log("NameByte:" + BitConverter.ToInt16(nameByte, 0) + Encoding.UTF8.GetString(nameByte, 2, nameByte.Length - 2));
        byte[] bodyByte = MsgBase.Encode(msgBase);
        Debug.Log("bodyByte:" + Encoding.UTF8.GetString(bodyByte));
        Int16 len = (Int16)(nameByte.Length + bodyByte.Length);
        //组装长度
        byte[] sendByte = new byte[len + 2];
        sendByte[0] = (byte)(len % 256);
        sendByte[1] = (byte)(len / 256);
        //组装名字
        Array.Copy(nameByte, 0, sendByte, 2, nameByte.Length);
        //组装消息体
        Array.Copy(bodyByte, 0, sendByte, 2 + nameByte.Length, bodyByte.Length);

        //进入消息队列
        ByteArray byteArray = new ByteArray(sendByte);
        int count = 0;
        lock (writeQueue)
        {
            writeQueue.Enqueue(byteArray);
            count = writeQueue.Count;
        }
        if (count == 1)
            socket.BeginSend(sendByte, 0, sendByte.Length, 0, SendCallback, socket);
    }

    private static void SendCallback(IAsyncResult asyncResult)
    {
        Socket socket = (Socket)asyncResult.AsyncState;
        if (socket == null || socket.Connected == false) return;
        int len = socket.EndSend(asyncResult);
        ByteArray ba = null;
        lock (writeQueue)
        {
            ba = writeQueue.Peek();
        }
        ba.readIndex += len;
        if (ba.length == 0)
        {
            lock (writeQueue)
            {
                writeQueue.Dequeue();
                if (writeQueue.Count > 0)
                    ba = writeQueue.Peek();
                else
                    ba = null;
            }
        }
        //当前消息未完全发送或者有下一条消息发送
        if (ba != null)
            socket.BeginSend(ba.data, ba.readIndex, ba.length, 0, SendCallback, socket);
        //已经没有消息发送并且现在是关闭连接状态
        else if (isClosing)
        {
            socket.Close();
        }
    }

    /// <summary>
    /// 关闭服务器连接
    /// </summary>
    public static void Close()
    {
        if (socket == null || socket.Connected == false) return;
        if (isConnecting) return;
        if (writeQueue.Count > 0)
            isClosing = true;
        else
        {
            socket.Close();
            FireEvent(NetEvent.Close, "关闭连接");
        }
    }

    private static void InitState()
    {
        socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        readBuff = new ByteArray();
        writeQueue = new Queue<ByteArray>();
        isConnecting = false;
        isClosing = false;

        msgQueue = new Queue<MessageToDo>();
        msgCount = 0;

        lastPingTime = Time.time;
        lastPongTime = Time.time;

        //添加Pong协议监听
        //if (msgListener.ContainsKey("MsgPong") == false)
        //{
        //    AddMsgListener("MsgPong", OnMsgPong);
        //}
    }

    private static void OnMsgPong(MsgBase msgBase)
    {
        lastPongTime = Time.time;
    }

    public static void AddMessageHander(ProtoID protoID, MessageHandler handler)
    {
        if (messageHandler.ContainsKey((int)protoID))
            messageHandler[(int)protoID] += handler;
        else
            messageHandler[(int)protoID] = handler;
    }

    public static void RemoveMessageHandler(ProtoID protoID, MessageHandler handler)
    {
        if (messageHandler.ContainsKey((int)protoID))
        {
            messageHandler[(int)protoID] -= handler;
            if (messageHandler[(int)protoID] == null)
                messageHandler.Remove((int)protoID);
        }
    }

    public static void AddEventListener(NetEvent netEvent, EventListener listener)
    {
        if (eventListeners.ContainsKey(netEvent))
            eventListeners[netEvent] += listener;
        else
            eventListeners[netEvent] = listener;
    }

    //public static void AddMsgListener(string msgName, MsgListener listener)
    //{
    //    if (msgListener.ContainsKey(msgName))
    //        msgListener[msgName] += listener;
    //    else
    //        msgListener[msgName] = listener;
    //}

    public static void RemoveEventListener(NetEvent netEvent, EventListener listener)
    {
        if (eventListeners.ContainsKey(netEvent))
        {
            eventListeners[netEvent] -= listener;
            if (eventListeners[netEvent] == null)
                eventListeners.Remove(netEvent);
        }
    }

    //public static void RemoveMsgListener(string msgName, MsgListener listener)
    //{
    //    if (msgListener.ContainsKey(msgName))
    //    {
    //        msgListener[msgName] -= listener;
    //        if (msgListener == null)
    //            msgListener.Remove(msgName);
    //    }
    //}

    //分发事件
    private static void FireEvent(NetEvent netEvent, string msg)
    {
        if (eventListeners.ContainsKey(netEvent))
        {
            eventListeners[netEvent](msg);
        }
        else
        {
            Debug.Log("分发事件时没有找到" + netEvent.ToString() + "对应的监听函数");
        }
    }

    ////分发消息
    //private static void FireMsg(string msgName, MsgBase msgBase)
    //{
    //    if (msgListener.ContainsKey(msgName))
    //        msgListener[msgName](msgBase);
    //}
}

public enum NetEvent
{
    ConnectSucc = 1,
    ConnectFail,
    Close
}