using Google.Protobuf;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpdateData<T>
{
    public T data;
    public string oldPK;
}
public abstract class BasePanel<T> : MonoBehaviour
{
    protected Transform content;
    protected int childNum = 0;//子物体数目(适配大小)
    protected float oneItemHeight;//一个物体的高度(用于视窗大小的自动适配)
    protected ReferenceCollector referenceCollector;
    protected GameObject Itemprefab;

    //现在所有有效数据
    protected Dictionary<string, T> dataDict = new Dictionary<string, T>();
    //需要向服务器添加的数据
    protected Dictionary<string, T> dataAddDict = new Dictionary<string, T>();
    //需要向服务器删除的数据
    protected List<string> dataDeleteList = new List<string>();
    //需要向服务器修改的数据
    protected Dictionary<string, UpdateData<T>> updateDataDict = new Dictionary<string, UpdateData<T>>();
    protected byte[] request;

    protected virtual void Awake()
    {
        referenceCollector = GetComponent<ReferenceCollector>();
        content = referenceCollector.Get<GameObject>("Content").transform;
        Itemprefab = referenceCollector.Get<GameObject>("ItemPrefab");
        referenceCollector.Get<GameObject>("BtnCreate").GetComponent<Button>().onClick.AddListener(CreateItemBtnCliced);
        GridLayoutGroup gridLayoutGroup = content.GetComponent<GridLayoutGroup>();
        oneItemHeight = gridLayoutGroup.cellSize.y + gridLayoutGroup.spacing.y;
        InitRequese();
    }

    /// <summary>
    /// 1.把请求协议转化为byte数组
    /// 2.设置请求对应的响应协议的回调函数
    /// </summary>
    protected abstract void InitRequese();
    /// <summary>
    /// 1.把服务端发送过来的消息转化为list数据组
    /// 2.填充有效数据字典dataDict
    /// </summary>
    protected abstract List<T> ResponseDecode(IMessage message);
    protected void ResponseHandler(IMessage message)
    {
        List<T> data = ResponseDecode(message);
        FixedContentSize();
        childNum = data.Count;
        //根据服务端发来的数据生成元素物体
        int i = 0;

        for (; i < data.Count && i < content.childCount; ++i)
        {
            BaseItem<T> item = content.GetChild(i).GetComponent<BaseItem<T>>();
            item.gameObject.SetActive(true);
            item.panel = this;
            item.UpdateItemData(data[i]);
            item.CheckBtnSwitch(false);
            item.isFirst = false;
        }
        if (i >= data.Count)
        {
            for (; i < content.childCount; ++i)
            {
                content.GetChild(i).gameObject.SetActive(false);
            }
        }
        else
        {
            for (; i < data.Count; ++i)
            {
                GameObject go = Instantiate(Itemprefab, content);
                BaseItem<T> item = content.GetChild(i).GetComponent<BaseItem<T>>();
                item.panel = this;
                item.UpdateItemData(data[i]);
                item.CheckBtnSwitch(false);
                item.isFirst = false;
            }
        }
    }
    protected void FixedContentSize()
    {
        Vector2 size = ((RectTransform)content).sizeDelta;
        size.y = childNum * oneItemHeight;
        ((RectTransform)content).sizeDelta = size;
    }

    protected void OnEnable()
    {
        NetManager.Send(request);
    }

    protected abstract void OnDisable();

    public bool AddToAddList(T item, string PK)
    {
        if (dataDict.ContainsKey(PK))
        {
            return false;
        }
        dataAddDict[PK] = item;
        dataDict[PK] = item;

        return true;
    }

    public void DeleteItem(string primaryKey)
    {
        --childNum;
        FixedContentSize();
        //防止这样一种情况:新增一个元素但是元素的数据都还没保存
        if (dataDict.ContainsKey(primaryKey))
        {
            dataDict.Remove(primaryKey);
            dataDeleteList.Add(primaryKey);
        }
        //如果删除的元素也是想增加的元素,就取消增加记录
        if (dataAddDict.ContainsKey(primaryKey))
            dataAddDict.Remove(primaryKey);
        //如果删除的元素也是想修改的元素,就取消修改记录(是else if)
        else if (updateDataDict.ContainsKey(primaryKey))
            updateDataDict.Remove(primaryKey);
    }

    public bool UpdateItem(T item, string PK, string lastPK, bool needCheck)
    {
        if (needCheck && dataDict.ContainsKey(PK))
            return false;
        dataDict.Remove(lastPK);
        dataDict[PK] = item;
        //如果更新的元素数据是新增列表里的，就直接更新新增列表对应数据
        //不再向修改列表增加数据
        if (dataAddDict.ContainsKey(lastPK))
        {
            dataAddDict.Remove(lastPK);
            dataAddDict[PK] = item;
        }
        else
        {
            if (updateDataDict.ContainsKey(lastPK))
            {
                UpdateData<T> updateItem = updateDataDict[lastPK];
                updateDataDict.Remove(lastPK);
                updateItem.data = item;
                updateDataDict[PK] = updateItem;
            }
            else
            {
                updateDataDict[PK] = new UpdateData<T>() { data = item, oldPK = lastPK };
            }
        }
        return true;
    }

    /// <summary>
    /// 创建一个新元素
    /// </summary>
    protected void CreateItemBtnCliced()
    {
        GameObject go = Instantiate(Itemprefab, content);
        ++childNum;
        FixedContentSize();
        BaseItem<T> item = go.GetComponent<BaseItem<T>>();
        item.panel = this;
        item.isEditor = true;
        item.CheckBtnSwitch(true);
    }
}
