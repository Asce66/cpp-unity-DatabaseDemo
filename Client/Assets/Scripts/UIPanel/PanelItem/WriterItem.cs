using Protocol;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WriterItem : MonoBehaviour
{
    ReferenceCollector referenceCollector;
    InputField txtBH, txtName, txtSex, txtBirth, txtDepart;
    Button writeBookBtn, WeiWenBtn, YanTaoBtn, delBtn;
    [HideInInspector]
    public WriterPanel writerPanel;
    Button checkBtn;

    public bool isEditor;//是否处于编辑模式
    public static readonly Color colorOK = Color.green;
    public static readonly Color colorEdito = Color.red;
    private string lastBH = "";//上一次的编号
    public bool isFirst = true;

    private void Awake()
    {
        referenceCollector = GetComponent<ReferenceCollector>();
        checkBtn = referenceCollector.Get<GameObject>("BtnOK").GetComponent<Button>();
        checkBtn.onClick.AddListener(BtnOkClicked);

        txtBH = referenceCollector.Get<GameObject>("BH").GetComponent<InputField>();
        txtName = referenceCollector.Get<GameObject>("Name").GetComponent<InputField>();
        txtSex = referenceCollector.Get<GameObject>("Sex").GetComponent<InputField>();
        txtBirth = referenceCollector.Get<GameObject>("Birth").GetComponent<InputField>();
        txtDepart = referenceCollector.Get<GameObject>("Depart").GetComponent<InputField>();

        writeBookBtn = referenceCollector.Get<GameObject>("BtnWriteBook").GetComponent<Button>();
        writeBookBtn.onClick.AddListener(WrtiterBooksBtnClicked);
        WeiWenBtn = referenceCollector.Get<GameObject>("BtnWeiWen").GetComponent<Button>();
        WeiWenBtn.onClick.AddListener(WeiWenBtnClicked);
        YanTaoBtn = referenceCollector.Get<GameObject>("BtnYanTao").GetComponent<Button>();
        YanTaoBtn.onClick.AddListener(YanTaoBtnClicked);
        delBtn = referenceCollector.Get<GameObject>("Btndelete").GetComponent<Button>();
        delBtn.onClick.AddListener(DeleteItem);
    }

    void DeleteItem()
    {
        if (isEditor == false)
            writerPanel.DeleteWriter(txtBH.text);
        Destroy(gameObject);
    }

    void WrtiterBooksBtnClicked()
    {
        writerPanel.ShowDropDown(txtBH.text, DropDownType.WriteBooks, writeBookBtn.transform.position);
    }

    void YanTaoBtnClicked()
    {
        writerPanel.ShowDropDown(txtBH.text, DropDownType.YanTao, YanTaoBtn.transform.position);

    }
    void WeiWenBtnClicked()
    {
        writerPanel.ShowDropDown(txtBH.text, DropDownType.WeiWen, WeiWenBtn.transform.position);
    }

    void BtnOkClicked()
    {
        //在修改状态下点击了按钮,即为需要提交数据
        if (isEditor)
        {
            Writer writer = new Writer()
            {
                BH = txtBH.text,
                Name = txtName.text,
                Sex = txtSex.text,
                Birth = txtBirth.text,
                BMH = txtDepart.text
            };
            bool canSave = false;
            if (isFirst)//第一次是新增元素
            {
                canSave = writerPanel.AddToAddList(writer);
                //如果不能插入这条数据，就让他一直保持isFirst,即为想插入状态
                isFirst = !canSave;
            }
            else//后面就是修改数据
            {
                //如果新的数据的主键是上一次的主键(即主键没有改变，但是可能改变了其他数据)
                //那就不检查主键是否重复，直接覆盖旧的数据
                canSave = writerPanel.UpdateWriter(writer, lastBH,!(writer.BH == lastBH));
            }
            if (canSave == false)
            {
                UIPanelMnr._Instance.ShowPrompt("作者编号重复");
            }
            else
            {
                EditorSwitch(false);
                isEditor = false;
            }
        }

        //在浏览模式点击按钮，即为想要修改数据
        else
        {
            lastBH = txtBH.text;
            EditorSwitch(true);
            isEditor = true;
        }
    }

    public void UpdateWriterData(Writer writer)
    {
        txtBH.text = writer.BH;
        txtBirth.text = writer.Birth;
        txtDepart.text = writer.BMH;
        txtName.text = writer.Name;
        txtSex.text = writer.Sex;
    }

    private void EditorSwitch(bool canEditor)
    {
        if (canEditor)
        {
            lastBH = txtBH.text;
            checkBtn.GetComponent<Image>().color = colorEdito;
        }
        else
            checkBtn.GetComponent<Image>().color = colorOK;
        Text fileText = null;
        InputField inputField;
        for (int i = 0; i < transform.childCount; ++i)
        {
            fileText = transform.GetChild(i).GetComponent<Text>();
            if (fileText != null)
            {
                inputField = fileText.gameObject.GetComponent<InputField>();
                inputField.enabled = canEditor;
            }
        }
    }

    public void CheckBtnSwitch(bool canEdit)
    {
        if (canEdit)
            checkBtn.GetComponent<Image>().color = colorEdito;
        else
            checkBtn.GetComponent<Image>().color = colorOK;
        EditorSwitch(canEdit);
    }
}
