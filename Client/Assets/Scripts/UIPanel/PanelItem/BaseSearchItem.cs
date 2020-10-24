using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class BaseSearchItem<T>: MonoBehaviour
{
    protected ReferenceCollector referenceCollector;
    protected Text selfTxt, otherTxt;

    protected void Awake()
    {
        InitReference();
    }
    protected  void InitReference()
    {
        referenceCollector = GetComponent<ReferenceCollector>();
        selfTxt = referenceCollector.Get<GameObject>("SelfTxt").GetComponent<Text>();
        otherTxt = referenceCollector.Get<GameObject>("OtherTxt").GetComponent<Text>();
    }
    public abstract void UpdateData(T data);
}
