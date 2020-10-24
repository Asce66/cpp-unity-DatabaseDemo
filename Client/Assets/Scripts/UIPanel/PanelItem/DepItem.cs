using Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DepItem : BaseItem<Department>
{
    protected override string alterError =>"部门编号重复";
    private InputField nameTxt;

    public override void UpdateItemData(Department item)
    {
        primaryKeyTxt.text = item.BMH;
        nameTxt.text = item.Name;
    }

    protected override void InitReference()
    {
        base.InitReference();
        primaryKeyTxt = referenceCollector.Get<GameObject>("BH").GetComponent<InputField>();
        nameTxt=referenceCollector.Get<GameObject>("Name").GetComponent<InputField>();
    }

    protected override bool AddToPanel(Department item)
    {
        return panel.AddToAddList(item, item.BMH);
    }

    protected override Department GetItem()
    {
        Department department = new Department()
        {
            BMH = primaryKeyTxt.text,
            Name = nameTxt.text
        };
        return department;
    }

    protected override bool UpdateToPanel(Department item)
    {
        return panel.UpdateItem(item, item.BMH, lastPrimaryKey, item.BMH != lastPrimaryKey);
    }
   
}
