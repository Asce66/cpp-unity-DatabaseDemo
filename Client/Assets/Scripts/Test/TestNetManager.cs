using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;
using System.Net;
using System;
using System.Text;
using System.Linq;

/// <summary>
/// 消息协议为"类名|身份,参数1，参数2......"
/// 比如"Move|127.0.0.1:8888,35.2,22,18.5"
/// </summary>
public static class TestNetManager
{
    //定义套接字
    static Socket socket;
    //数据缓存
    static ByteArray readBuff = new ByteArray();
    //监听消息的委托
    public delegate void MsgListener(string message);
    //消息处理的委托数组
    private static Dictionary<string, MsgListener> listeners = new Dictionary<string, MsgListener>();
    //消息接收队列
    static Queue<string> msgQueue = new Queue<string>();
    //数据发送队列
    static Queue<ByteArray> writeQueue = new Queue<ByteArray>();

    //服务器IP地址
    const string SERVERIP = "127.0.0.1";
    //服务器端口号
    const int SERVERPORT = 8888;
    //是否处于关闭状态
    static bool isClosing = false;

    /// <summary>
    /// 添加监听
    /// </summary>
    public static void AddListener(string msgName, MsgListener listener)
    {
        listeners[msgName] = listener;
    }

    /// <summary>
    /// 获取描述（IP+端口号）
    /// </summary>
    /// <returns></returns>
    public static string GetDesc()
    {
        if (socket == null || socket.Connected == false)
            return "";
        return socket.LocalEndPoint.ToString();
    }

    /// <summary>
    /// 连接服务器
    /// </summary>
    public static void Connect()
    {
        socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        IPAddress address = IPAddress.Parse(SERVERIP);
        IPEndPoint iPEndPoint = new IPEndPoint(address, SERVERPORT);
        socket.Connect(SERVERIP, SERVERPORT);
        socket.BeginReceive(readBuff.data, readBuff.writeIndex, readBuff.remain, 0, ReceiveCallback, socket);
        //这里不要用异步，因为外部在连接后就会直接访问socket
        //异步可能还未完成连接，但是又访问socket，造成空指针
        // socket.BeginConnect(iPEndPoint, ConnectCallback, socket);
    }

    /// <summary>
    /// 发送消息
    /// </summary>
    public static void Send(string message)
    {
        //如果现在处于关闭状态
        //就不再发送新的消息
        //只是把所有发送消息队列的消息发送出去
        if (isClosing) return;
        if (socket == null || socket.Connected == false) return;
        byte[] dataContent = Encoding.Default.GetBytes(message);
        Int16 len = (Int16)dataContent.Length;
        byte[] dataLen = BitConverter.GetBytes(len);
        //统一规定使用小端口模式
        if (BitConverter.IsLittleEndian == false)
        {
            dataLen.Reverse();
        }
        byte[] data = dataLen.Concat(dataContent).ToArray();
        Debug.Log("发送数据" + message);
        ByteArray ba = new ByteArray(data);
        int count = 0;
        lock (writeQueue)//数据队列加锁，实现线程安全（所以我们用count而不是直接queue.Count）
        {
            writeQueue.Enqueue(ba);
            count = writeQueue.Count;
        }

        ///<summary>
        ///这里必须保证队列里只有一条消息才发送
        ///因为后面是异步发送消息，如果不限制只发送出去队列里面的一条消息
        ///发送出去的多条消息无法确定是哪一条消息先发送完毕
        ///那么在回调函数中进行消息是否发送完成的验证就无法正确进行
        ///如果队列里面的一条消息还没发送完成又来了其他消息这里不会立即发送
        ///会在回调函数中验证当前发送消息是否完整发出再进行下一条消息的发送
        ///</summary>
        if (count == 1)
        {
            socket.BeginSend(ba.data, ba.readIndex, ba.length, 0, SendCallback, socket);
        }
    }

    private static void SendCallback(IAsyncResult asyncResult)
    {
        Socket socket = (Socket)asyncResult.AsyncState;
        int len = socket.EndSend(asyncResult);
        //判断发送的数据是否已经完整发送出去
        ByteArray ba;
        lock (writeQueue)
        {
            ba = writeQueue.First();
        }
        ba.readIndex += len;
        if (ba.length == 0)//已经发送完成,则发送消息队列下一条消息
        {
            lock (writeQueue)
            {
                writeQueue.Dequeue();
                ba = writeQueue.First();
            }
        }
        //发送不完整（我们已经移动了读数据(readIndex)的下标，直接继续发送即可）
        //或者发送完整，继续发送下一条消息
        if (ba != null)
        {
            socket.BeginSend(ba.data, ba.readIndex, ba.length, 0, SendCallback, socket);
        }
        //消息队列已经没有需要发送消息
        //而且现在是需要关闭连接的状态
        else if(isClosing)
        {
            socket.Close();
        }
    }

    /// <summary>
    /// 每一帧处理消息队列的消息
    /// </summary>
    public static void Update()
    {
        if (msgQueue.Count <= 0) return;

        string msg = msgQueue.Dequeue();
        string[] split = msg.Split('|');
        string msgName = split[0];
        string msgArgs = split[1];
        if (listeners.ContainsKey(msgName))
            listeners[msgName](msgArgs);
    }

    private static void ConnectCallback(IAsyncResult asyncResult)
    {
        Socket socket = (Socket)asyncResult.AsyncState;
        socket.EndConnect(asyncResult);
        socket.BeginReceive(readBuff.data, readBuff.writeIndex, readBuff.remain, 0, ReceiveCallback, socket);
    }

    private static void ReceiveCallback(IAsyncResult asyncResult)
    {
        try
        {
            Socket socket = (Socket)asyncResult.AsyncState;
            int len = socket.EndReceive(asyncResult);
            if(len<=0)
            {
                Close();
                return;
            }
            readBuff.writeIndex += len;
            OnRevceiveData();

            readBuff.CheckAndMoveData();
            readBuff.Resize(readBuff.length * 2);

            socket.BeginReceive(readBuff.data, readBuff.writeIndex, readBuff.remain, 0, ReceiveCallback, socket);
        }
        catch (Exception ex)
        {
            Debug.LogError("接受数据错误" + ex.ToString());
        }
    }

    /// <summary>
    /// 处理缓存中的数据
    /// </summary>
    private static void OnRevceiveData()
    {
        //2即为存储长度的空间
        while (readBuff.length > 2)
        {
            //判断数据是否长度足够
            Int16 count = (Int16)(readBuff.data[readBuff.readIndex] | (readBuff.data[readBuff.readIndex + 1] << 8));
            if (readBuff.length < 2 + count)
                break;

            readBuff.readIndex += 2;//先去除长度信息
            byte[] data = readBuff.ReadData(readBuff.readIndex, count);
            Debug.Log("数据：：" + data.ToString());
            string msg = Encoding.UTF8.GetString(data);
            msgQueue.Enqueue(msg);
            Debug.Log("接收到数据" + msg);
        }
    }

    public static void Close()
    {
        if (writeQueue.Count > 0)
            isClosing = true;
        else
            socket.Close();
    }

}
