using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Profiling;

public class Main : MonoBehaviour
{
    [SerializeField]
    GameObject humanPrefab;//角色预制体
    BaseHuman myHuman;//本地角色
    Dictionary<string, BaseHuman> otherHuman = new Dictionary<string, BaseHuman>();//其他玩家

    void SendForList()
    {
        TestNetManager.Send("List|");
    }

    private void Awake()
    {
        //网络模块
        TestNetManager.AddListener("Enter", OnEnter);
        TestNetManager.AddListener("List", OnList);
        TestNetManager.AddListener("Move", OnMove);
        TestNetManager.AddListener("Leave", OnLeave);
        TestNetManager.AddListener("Attack", OnAttack);
        TestNetManager.AddListener("Die", OnDie);
        TestNetManager.Connect();
        //生成本地角色
        GameObject go = Instantiate<GameObject>(humanPrefab);
        float x = Random.Range(-5, 5);
        float z = Random.Range(-5, 5);
        go.transform.position = new Vector3(x, 0, z);
        myHuman = go.AddComponent<CtrlHuman>();
        myHuman.desc = TestNetManager.GetDesc();

        //实例化本地玩家    
        Vector3 pos = go.transform.position;
        float euly = go.transform.eulerAngles.y;
        StringBuilder sb = new StringBuilder("Enter|");
        sb.Append(TestNetManager.GetDesc() + ",");
        sb.Append(pos.x + ",");
        sb.Append(pos.y + ",");
        sb.Append(pos.z + ",");
        sb.Append(euly + "," + 100.ToString());
        //发送Enter协议  
        string message = sb.ToString();
        TestNetManager.Send(message);
        TestNetManager.Send("List|");
        //Invoke("SendForList", 0.5f);
    }

    private void Start()
    {
        //发送请求其他玩家协议
        //NetManager.Send("List|");
    }

    private void Update()
    {
        TestNetManager.Update();
    }

    public void OnEnter(string msg)
    {
        Debug.Log("OnEnter" + msg);
        string[] split = msg.Split(',');
        string desc = split[0];
        if (desc == myHuman.desc)//如果是本地玩家就忽略
            return;
        //解析位置信息
        float x = float.Parse(split[1]);
        float y = float.Parse(split[2]);
        float z = float.Parse(split[3]);
        float elury = float.Parse(split[4]);
        int hp = int.Parse(split[5]);
        //实例化玩家
        GameObject go = Instantiate<GameObject>(humanPrefab);
        go.transform.position = new Vector3(x, y, z);
        go.transform.eulerAngles = new Vector3(0, elury, 0);
        go.AddComponent<SyncHuman>().desc = desc;
        otherHuman[desc] = go.GetComponent<SyncHuman>();
    }

    public void OnList(string msg)
    {
        string[] split = msg.Split(',');
        int count = (split.Length - 1) / 6;
        for (int i = 0; i < count; i++)
        {
            string desc = split[i * 6];
            if (desc == myHuman.desc) continue;
            //解析数据
            float x = float.Parse(split[i * 6 + 1]);
            float y = float.Parse(split[i * 6 + 2]);
            float z = float.Parse(split[i * 6 + 3]);
            float elury = float.Parse(split[i * 6 + 4]);
            int hp = int.Parse(split[i * 6 + 5]);
            GameObject go = Instantiate<GameObject>(humanPrefab);
            SyncHuman human = go.AddComponent<SyncHuman>();
            human.desc = desc;
            go.transform.position = new Vector3(x, y, z);
            go.transform.eulerAngles = new Vector3(0, elury, 0);
            otherHuman[desc] = human;
        }

    }

    public void OnMove(string msg)
    {
        Debug.Log("OnMove" + msg);
        string[] split = msg.Split(',');
        float x = float.Parse(split[1]);
        float y = float.Parse(split[2]);
        float z = float.Parse(split[3]);
        string desc = split[0];
        Vector3 position = new Vector3(x, y, z);
        if (desc == myHuman.desc)
            myHuman.MoveTo(position);
        else
        {
            if (otherHuman.ContainsKey(desc))
                otherHuman[desc].MoveTo(position);
        }
    }

    public void OnLeave(string msg)
    {
        Debug.Log("OnLeave" + msg);
        string desc = msg;
        if (otherHuman.ContainsKey(desc))
        {
            GameObject.Destroy(otherHuman[desc].gameObject);
            otherHuman.Remove(desc);
        }
    }

    public void OnAttack(string msg)
    {
        Debug.Log("OnAttack" + msg);
        string[] split = msg.Split(',');
        if (otherHuman.ContainsKey(split[0]) == false)
            return;
        float eury = float.Parse(split[1]);
        ((SyncHuman)otherHuman[split[0]]).SynAttack(eury);
    }

    public void OnDie(string msg)
    {
        
        Debug.Log("oneDie"+ msg);
        string desc = msg;
        if(desc==myHuman.desc)
        {
            Debug.Log("你的角色已经阵亡了");
            Destroy(myHuman);
        }
        else
        {
            if(otherHuman.ContainsKey(desc))
            {
                Destroy(otherHuman[desc].gameObject);
                otherHuman.Remove(desc);
            }
        }
    }
}
