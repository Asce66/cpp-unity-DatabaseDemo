using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class test : MonoBehaviour
{
    public InputField iDinput;
    public InputField pWinput;
    public InputField textinput;

    void Start()
    {
        NetManager.AddEventListener(NetEvent.ConnectSucc, ConnectSucc);
        NetManager.AddEventListener(NetEvent.ConnectFail, ConnectFail);
        NetManager.AddEventListener(NetEvent.Close, OnClose);
        //NetManager.AddMsgListener("MsgMove", OnMsgMove);
        //NetManager.AddMsgListener("MsgLogin", OnMsgLogin);
        //NetManager.AddMsgListener("MsgRegister", OnMsgRegister);
        //NetManager.AddMsgListener("MsgKick", OnMsgKick);
        //NetManager.AddMsgListener("MsgSaveText", OnMsgSaveText);
        //NetManager.AddMsgListener("MsgGetText", OnMsgGetText);     
    }

    public void OnSaveTextClick()
    {
        MsgSaveText msgSaveText = new MsgSaveText();
        msgSaveText.text = textinput.text;
        NetManager.Send(msgSaveText);
    }
     
    public void OnRegisterClick()
    {
        MsgRegister msgRegister = new MsgRegister();
        msgRegister.id = iDinput.text;
        msgRegister.pw = pWinput.text;
        NetManager.Send(msgRegister);
    }

    public void OnLoginClick()
    {
        MsgLogin msgRegister = new MsgLogin();
        msgRegister.id = iDinput.text;
        msgRegister.pw = pWinput.text;
        NetManager.Send(msgRegister);
    }

    private void OnMsgGetText(MsgBase msgBase)
    {
        MsgGetText msgGetText = (MsgGetText)msgBase;
        textinput.text = msgGetText.text;
    }

    private void OnMsgSaveText(MsgBase msgBase)
    {
        MsgSaveText msgSaveText = (MsgSaveText)msgBase;
        if (msgSaveText.result == 0)
        {
            Debug.Log("保存成功");
        }
        else
            Debug.Log("保存失败");
    }

    private void OnMsgKick(MsgBase msgBase)
    {
        Debug.Log("踢下线");
    }

    private void OnMsgRegister(MsgBase msgBase)
    {
        MsgRegister msgRegister = (MsgRegister)msgBase;
        if (msgRegister.result == 0)
            Debug.Log("注册成功");
        else
            Debug.Log("注册失败");
    }

    private void OnMsgLogin(MsgBase msgBase)
    {
        MsgLogin mSgLogin = (MsgLogin)msgBase;
        if(mSgLogin.result==0)
        {
            Debug.Log("登录成功");
            MsgGetText msgGetText = new MsgGetText();
            NetManager.Send(msgGetText);
        }
        else
        {
            Debug.Log("登录失败");
        }
    }

    private void Update()
    {
        NetManager.Update();
    }

    public void OnMsgMove(MsgBase msgBase)
    {
        MsgMove msgMove = (MsgMove)msgBase;
        Debug.Log("受到移动消息：" + msgMove.x + " " + msgMove.y + " " + msgMove.z);
    }

    public void Close()
    {
        NetManager.Close();
    }

    void ConnectSucc(string str)
    {
        Debug.Log("连接成功");
    }

    void ConnectFail(string str)
    {

    }

    public void OnMove()
    {
        MsgMove msgMove = new MsgMove();
        msgMove.x = 1;
        msgMove.z = 13;
        NetManager.Send(msgMove);

    }

    void OnClose(string str)
    {
        Debug.Log(str);
    }

    public void Connect()
    {
        NetManager.Connect("127.0.0.1", 6688);
    }
}
