using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActivitiesPanel : MonoBehaviour
{
    private ReferenceCollector referenceCollector;
    Text activityTxt;
    //慰问 出书 研讨的顺序排列
    readonly string[] activityStr = new string[] { "慰问名称", "书籍编号", "研讨会名称" };
    GameObject[] panels;

    private void Awake()
    {
        panels = new GameObject[3];
        referenceCollector = GetComponent<ReferenceCollector>();
        panels[0] = referenceCollector.Get<GameObject>("DoWeiWenHolder");
        panels[1] = referenceCollector.Get<GameObject>("WriteBookHolder");
        panels[2] = referenceCollector.Get<GameObject>("DoYanTaoHolder");
        activityTxt = referenceCollector.Get<GameObject>("Text1").GetComponent<Text>();
        referenceCollector.Get<GameObject>("BtnWeiWen").GetComponent<Button>().onClick.AddListener(() => SwitchPanel(0));
        referenceCollector.Get<GameObject>("btnWriteBook").GetComponent<Button>().onClick.AddListener(() => SwitchPanel(1));
        referenceCollector.Get<GameObject>("BtnYanTao").GetComponent<Button>().onClick.AddListener(() => SwitchPanel(2));
    }

    private void SwitchPanel(int index)
    {
        activityTxt.text = activityStr[index];
        for (int i = 0; i < panels.Length; ++i)
        {
            if (i != index)
                panels[i].SetActive(false);
            else
                panels[i].SetActive(true);
        }
    }
}
