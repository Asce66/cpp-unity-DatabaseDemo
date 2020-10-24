using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

public class TestNet : MonoBehaviour
{
    public string ip;
    public int port;
    private void Awake()
    {
        Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        socket.Connect(new IPEndPoint(IPAddress.Parse(ip), port));
        Debug.Log("连接成功了");
    }
}
