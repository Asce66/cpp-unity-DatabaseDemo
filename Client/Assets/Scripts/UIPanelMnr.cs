using Google.Protobuf.WellKnownTypes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

public enum PanelType
{
    PanelNull = -1,
    PanelLogin,
    PanelMainMenu,
    PanelWriter,
    PanelDepartment,
    PanelBook,
    PanelWeiWen,
    PanelYanTao,
    PanelActivities
}

public class UIPanelMnr : MonoBehaviour
{

    private static UIPanelMnr instance;
    public static UIPanelMnr _Instance
    {
        get
        {
            if (instance == null)
            {
                instance = GameObject.FindObjectOfType<UIPanelMnr>();
            }

            return instance;
        }
    }

    private Dictionary<int, GameObject> panelDict = new Dictionary<int, GameObject>();

    private PanelType nowPanelType = PanelType.PanelNull;

    [SerializeField] GameObject worldPrompt;
    private Text promptTxt;

    private void Awake()
    {
        promptTxt = worldPrompt.GetComponentInChildren<Text>();
        HidePrompt();
    }

    public void TransactionPanel(PanelType panelType)
    {
        if (nowPanelType == panelType)
            return;
        if (panelDict.ContainsKey((int)panelType) == false)
        {
            GameObject go = Resources.Load<GameObject>("Panel/" + panelType.ToString());
            GameObject panel = Instantiate(go, transform);
            if (panel == null)
            {
                Debug.LogError("不存在Panel：" + panelType.ToString());
                return;
            }
            panelDict[(int)panelType] = panel;
        }
        if (nowPanelType != PanelType.PanelNull)
        {
            panelDict[(int)nowPanelType].SetActive(false);
        }

        nowPanelType = panelType;
        panelDict[(int)nowPanelType].SetActive(true);
    }

    public void ShowPrompt(string str)
    {
        worldPrompt.SetActive(true);
        promptTxt.text = str;
        Invoke("HidePrompt", 3);
    }

    void HidePrompt()
    {
        worldPrompt.SetActive(false);
    }
}
