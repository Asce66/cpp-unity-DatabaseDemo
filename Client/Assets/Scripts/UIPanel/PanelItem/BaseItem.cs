using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class BaseItem<T> : MonoBehaviour
{
    protected ReferenceCollector referenceCollector;

    protected Button checkBtn;
    public static readonly Color colorOK = Color.green;
    public static readonly Color colorEdito = Color.red;
    [HideInInspector]
    public bool isEditor;
    protected string lastPrimaryKey = "";
    [HideInInspector]
    public bool isFirst = true;
    protected abstract string alterError { get; }
    protected InputField primaryKeyTxt;
    public BasePanel<T> panel;

    protected virtual void Awake()
    {
        InitReference();
    }

    protected virtual void InitReference()
    {
        referenceCollector = GetComponent<ReferenceCollector>();
        checkBtn = referenceCollector.Get<GameObject>("CheckBtn").GetComponent<Button>();
        checkBtn.onClick.AddListener(BtnOkClicked);
        referenceCollector.Get<GameObject>("DeleteBtn").GetComponent<Button>().onClick.AddListener(DeleteItem);
    }

    protected void DeleteItem()
    {
        //如果是第一次新建，即为数据还没保存，就不需要清除存放的数据
        if (isFirst == false)
        {
            if (isEditor == false)//如果是编辑模式，就传递普通模式记录的主键(防止编辑模式又改了主键,导致传过去的
                                  //主键还没被保存，就无法删除原本的数据)
            {
                panel.DeleteItem(primaryKeyTxt.text);
            }
            else
            {
                panel.DeleteItem(lastPrimaryKey);
            }
        }
        Destroy(gameObject);
    }

    protected abstract T GetItem();

    protected void BtnOkClicked()
    {
        //在修改状态下点击了按钮,即为需要提交数据
        if (isEditor)
        {
            T item = GetItem();
            bool canSave = false;
            if (isFirst)//第一次是新增元素
            {
                canSave = AddToPanel(item);
                //如果不能插入这条数据，就让他一直保持isFirst,即为想插入状态
                isFirst = !canSave;
            }
            else//后面就是修改数据
            {
                //如果新的数据的主键是上一次的主键(即主键没有改变，但是可能改变了其他数据)
                //那就不检查主键是否重复，直接覆盖旧的数据
                canSave = UpdateToPanel(item);
            }
            if (canSave == false)
            {
                UIPanelMnr._Instance.ShowPrompt(alterError);
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
            lastPrimaryKey = primaryKeyTxt.text;
            EditorSwitch(true);
            isEditor = true;
        }
    }

    protected abstract bool AddToPanel(T item);

    protected abstract bool UpdateToPanel(T item);

    public abstract void UpdateItemData(T item);

    protected void EditorSwitch(bool canEditor)
    {
        if (canEditor)
        {
            lastPrimaryKey = primaryKeyTxt.text;
            checkBtn.GetComponent<Image>().color = colorEdito;
        }
        else
            checkBtn.GetComponent<Image>().color = colorOK;
        InputField inputField;
        for (int i = 0; i < transform.childCount; ++i)
        {
            inputField = transform.GetChild(i).GetComponent<InputField>();
            if (inputField != null)
            {
                inputField.enabled = canEditor;
            }
        }
    }

    public void CheckBtnSwitch(bool canEditor)
    {
        if (canEditor)
            checkBtn.GetComponent<Image>().color = colorEdito;
        else
            checkBtn.GetComponent<Image>().color = colorOK;
        EditorSwitch(canEditor);
    }

}
