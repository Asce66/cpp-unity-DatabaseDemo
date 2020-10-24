using Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class YanTaoItem : BaseItem<YanTao>
{
    private InputField addressTxt, timeTxt, contentTxt;
    protected override string alterError => "研讨会的名称重复";

    protected override void InitReference()
    {
        base.InitReference();
        primaryKeyTxt = referenceCollector.Get<GameObject>("Name").GetComponent<InputField>();
        addressTxt = referenceCollector.Get<GameObject>("Address").GetComponent<InputField>();
        timeTxt = referenceCollector.Get<GameObject>("Time").GetComponent<InputField>();
        contentTxt = referenceCollector.Get<GameObject>("Content").GetComponent<InputField>();
    }

    public override void UpdateItemData(YanTao item)
    {
        primaryKeyTxt.text = item.Name;
        addressTxt.text = item.Address;
        timeTxt.text = item.Time;
        contentTxt.text = item.Content;
    }

    protected override bool AddToPanel(YanTao item)
    {
        return panel.AddToAddList(item, item.Name);
    }

    protected override YanTao GetItem()
    {
        YanTao yanTao = new YanTao()
        {
            Address = addressTxt.text,
            Name = primaryKeyTxt.text,
            Time = timeTxt.text,
            Content = contentTxt.text
        };
        return yanTao;
    }

    protected override bool UpdateToPanel(YanTao item)
    {
        return panel.UpdateItem(item, item.Name, lastPrimaryKey, !(lastPrimaryKey == item.Name));
    }
}
