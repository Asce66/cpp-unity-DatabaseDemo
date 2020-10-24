using Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeiWenItem : BaseItem<WeiWen>
{
    protected override string alterError => "慰问名称重复";
    private InputField addressTxt, timeTxt;

    public override void UpdateItemData(WeiWen item)
    {
        primaryKeyTxt.text = item.Name;
        addressTxt.text = item.Address;
        timeTxt.text = item.Time;
    }
    protected override void InitReference()
    {
        base.InitReference();
        primaryKeyTxt = referenceCollector.Get<GameObject>("Name").GetComponent<InputField>();
        addressTxt = referenceCollector.Get<GameObject>("Address").GetComponent<InputField>();
        timeTxt = referenceCollector.Get<GameObject>("Time").GetComponent<InputField>();
    }
    protected override bool AddToPanel(WeiWen item)
    {
        return panel.AddToAddList(item, item.Name);
    }

    protected override WeiWen GetItem()
    {
        WeiWen weiWen = new WeiWen()
        {
            Name=primaryKeyTxt.text,
            Address=addressTxt.text,
            Time=timeTxt.text
        };
        return weiWen;
    }

    protected override bool UpdateToPanel(WeiWen item)
    {
        return panel.UpdateItem(item, item.Name, lastPrimaryKey, lastPrimaryKey != item.Name);
    }
}
