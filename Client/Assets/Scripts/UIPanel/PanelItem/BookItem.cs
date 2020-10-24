using Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BookItem : BaseItem<Book>
{
    private InputField nameTxt, timeTxt, priceTxt;
    protected override string alterError => "作者编号重复";

    protected override void InitReference()
    {
        base.InitReference();
        primaryKeyTxt = referenceCollector.Get<GameObject>("BH").GetComponent<InputField>();
        nameTxt = referenceCollector.Get<GameObject>("Name").GetComponent<InputField>();
        timeTxt = referenceCollector.Get<GameObject>("Time").GetComponent<InputField>();
        priceTxt = referenceCollector.Get<GameObject>("Price").GetComponent<InputField>();
    }

    public override void UpdateItemData(Book item)
    {
        priceTxt.text = item.Price.ToString();
        primaryKeyTxt.text = item.BH;
        nameTxt.text = item.Name;
        timeTxt.text = item.Time;
    }

    protected override bool AddToPanel(Book item)
    {
        return panel.AddToAddList(item, item.BH);
    }

    protected override Book GetItem()
    {
        Book book = new Book()
        {
            BH = primaryKeyTxt.text,
            Time = timeTxt.text,
            Name = nameTxt.text,         
        };
        float price = 0;
        float.TryParse(priceTxt.text, out price);
        book.Price = price;
        return book;
    }

    protected override bool UpdateToPanel(Book item)
    {
        return panel.UpdateItem(item, item.BH, lastPrimaryKey, !(item.BH == lastPrimaryKey));
    }

}
