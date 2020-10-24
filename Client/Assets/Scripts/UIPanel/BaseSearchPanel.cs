using Google.Protobuf;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class BaseSearchPanel<T> : MonoBehaviour
{
    protected ReferenceCollector referenceCollector;
    protected byte[] request;
    protected GameObject itemPrefab;
    protected Transform content;
    protected float oneItemHeight;

    protected virtual void Awake()
    {
        referenceCollector = GetComponent<ReferenceCollector>();
        itemPrefab = referenceCollector.Get<GameObject>("Activity");
        content = referenceCollector.Get<GameObject>("Content").transform;
        GridLayoutGroup gridLayoutGroup = content.GetComponent<GridLayoutGroup>();
        oneItemHeight = gridLayoutGroup.cellSize.y + gridLayoutGroup.spacing.y;
    }

    protected void OnEnable()
    {
        NetManager.Send(request);
    }

    protected abstract List<T> HandleMessage(IMessage message);
    protected void SpawnChilds(IMessage message)
    {
        List<T> data = HandleMessage(message);
        int i = 0;
        Vector2 size = content.GetComponent<RectTransform>().sizeDelta;
        size.y = data.Count * oneItemHeight;
        content.GetComponent<RectTransform>().sizeDelta = size;
        for (; i < data.Count && i < content.childCount; ++i)
        {
            content.GetChild(i).gameObject.SetActive(true);
            content.GetChild(i).GetComponent<BaseSearchItem<T>>().UpdateData(data[i]);
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
                GameObject item = Instantiate(itemPrefab, content);
                item.GetComponent<BaseSearchItem<T>>().UpdateData(data[i]);
            }
        }
    }
}

