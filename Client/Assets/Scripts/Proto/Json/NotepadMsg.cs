using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class MsgSaveText:MsgBase
{
    public MsgSaveText()
    {
        protoName = "MsgSaveText";
    }
    public string text;
    //结果 0-成功 1-失败,文本太长无法保存
    public int result = 0;
}

public class MsgGetText:MsgBase
{
    public MsgGetText()
    {
        protoName = "MsgGetText";
    }

    public string text = "";
}
