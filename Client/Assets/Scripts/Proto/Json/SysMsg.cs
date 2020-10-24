using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;

public class MsgPing:MsgBase
{
    public MsgPing()
    {
        protoName = "MsgPing";
    }
}

public class MsgPong:MsgBase
{
    public MsgPong()
    {
        protoName = "MsgPong";
    }
}
