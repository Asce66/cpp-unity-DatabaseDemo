using Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WriteBookItem : BaseSearchItem<WriteBook>
{
    public override void UpdateData(WriteBook data)
    {
        selfTxt.text = data.BBH;
        otherTxt.text = data.SBH;
    }
}
