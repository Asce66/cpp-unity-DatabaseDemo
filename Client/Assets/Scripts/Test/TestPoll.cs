using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine.UI;

public class TestPoll : MonoBehaviour
{
    Byte[] data = new Byte[1024];
    [SerializeField] InputField inputField;
    [SerializeField] Text prompt;
    string promptStr = "";

    Socket socket;

    private void Update()
    {
        if (socket == null) return;
        if(socket.Poll(0,SelectMode.SelectRead))
        {
            int len = socket.Receive(data, 0, data.Length,0);
            prompt.text = Encoding.UTF8.GetString(data, 0, len);
        }
    }

    void ConnectCallback(IAsyncResult result)
    {
        try
        {
            Socket socket = result.AsyncState as Socket;
            socket.EndConnect(result);
            promptStr = "连接成功";
            Byte[] sendData = Encoding.UTF8.GetBytes("helloServer");

            socket.BeginSend(sendData, 0, sendData.Length, 0, SendCallback, socket);         
        }
        catch (Exception e)
        {
            Debug.LogError("连接失败 " + e.ToString());
        }
    }

    //void ReceiveCallback(IAsyncResult result)
    //{
    //    try
    //    {
    //        Socket socket = (Socket)result.AsyncState;
    //        int len = socket.EndReceive(result);

    //        string message;
    //        message = Encoding.UTF8.GetString(data, 0, len);
    //       promptStr= "受到消息 " + message;
    //        socket.BeginReceive(data, 0, data.Length, 0, ReceiveCallback, socket);
    //    }
    //    catch (Exception ex)
    //    {
    //        Debug.LogError("接收数据错误" + ex.ToString());
    //    }
    //}

    public void SendMessage()
    {
        string message = inputField.text;
        Byte[] data = Encoding.UTF8.GetBytes(message);
        socket.BeginSend(data,0,data.Length,0,SendCallback,socket);
    }

    void SendCallback(IAsyncResult result)
    {
        try
        {
            Socket socket = (Socket)result.AsyncState;
            int len = socket.EndSend(result);
            Debug.Log("发送一条数据成功，长度为" + len);
        }
        catch (Exception ex)
        {
            Debug.LogError("发送数据错误" + ex.ToString());
        }
    }

    public void ConnectServer()
    {
        socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        IPEndPoint iPEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 8888);
        socket.BeginConnect(iPEndPoint, ConnectCallback, socket);
    }

}
