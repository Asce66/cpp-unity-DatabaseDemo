using Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DoYanTaoItem : BaseSearchItem<DoYanTao>
{
    public override void UpdateData(DoYanTao data)
    {
        selfTxt.text = data.Name;
        otherTxt.text = data.SBH;
    }
}

   
