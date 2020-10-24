using Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DoWeiWenItem : BaseSearchItem<DoWeiWen>
{
    public override void UpdateData(DoWeiWen data)
    {
        selfTxt.text = data.Name;
        otherTxt.text = data.SBH;
    }
}
