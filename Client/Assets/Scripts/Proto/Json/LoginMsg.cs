using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class MsgRegister : MsgBase
{
    public MsgRegister()
    {
        protoName = "MsgRegister";
    }

    public string id, pw;
    //服务端发回，0代表成功 1代表失败
    public int result;
}

public class MsgLogin : MsgBase
{
    public MsgLogin()
    {
        protoName = "MsgLogin";
    }
    public string id, pw;
    //服务端发回，0代表成功 1代表失败
    public int result;
}

//踢下线消息(服务端发送)
public class MsgKick : MsgBase
{
    public MsgKick()
    {
        protoName = "MsgKick";
    }  
    //原因 0-其他人登录
    public int reason = 0;
}

