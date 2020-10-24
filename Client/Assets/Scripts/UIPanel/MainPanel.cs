using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainPanel : MonoBehaviour
{
    private ReferenceCollector referenceCollector;
    private Button btnWriter;
    private Button btnDepartment;
    private Button btnBook;
    private Button btnActivity;

    private void Awake()
    {
        referenceCollector = GetComponentInChildren<ReferenceCollector>();
        referenceCollector.Get<GameObject>("Writer").GetComponent<Button>()
            .onClick.AddListener(() =>
            {
                UIPanelMnr._Instance.TransactionPanel
                (PanelType.PanelWriter);
            });
        referenceCollector.Get<GameObject>("Department").GetComponent<Button>()
            .onClick.AddListener(() =>
            {
                UIPanelMnr._Instance.TransactionPanel
                (PanelType.PanelDepartment);
            });
        referenceCollector.Get<GameObject>("Book").GetComponent<Button>()
            .onClick.AddListener(() =>
            {
                UIPanelMnr._Instance.TransactionPanel
                (PanelType.PanelBook);
            });
        referenceCollector.Get<GameObject>("WeiWen").GetComponent<Button>()
            .onClick.AddListener(() =>
            {
                UIPanelMnr._Instance.TransactionPanel
                (PanelType.PanelWeiWen);
            });
        referenceCollector.Get<GameObject>("YanTao").GetComponent<Button>()
            .onClick.AddListener(() =>
            {
                UIPanelMnr._Instance.TransactionPanel
                (PanelType.PanelYanTao);
            });
        referenceCollector.Get<GameObject>("Activity").GetComponent<Button>()
            .onClick.AddListener(() =>
            {
                UIPanelMnr._Instance.TransactionPanel
                (PanelType.PanelActivities);
            });
    }
}
