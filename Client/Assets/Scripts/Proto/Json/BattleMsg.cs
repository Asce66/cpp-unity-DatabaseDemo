using System;
using System.Collections.Generic;
using System.Text;

public class MsgMove : MsgBase
{
    public MsgMove() { protoName = "MsgMove"; }

    public float x, y, z;
}
