using Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameMnr : MonoBehaviour
{
    [SerializeField] Button returnMainMenu;
    public static bool isLogin { get; private set; } = false;
    [SerializeField]
    string IP;
    [SerializeField]
    int  Port;
    private void Awake()
    {  
        UIPanelMnr uIPanelMnr = UIPanelMnr._Instance;
        NetManager.AddEventListener(NetEvent.ConnectSucc, (str) => { uIPanelMnr.TransactionPanel(PanelType.PanelLogin); });
        NetManager.AddMessageHander(ProtoID.S2CLogin, (message) =>
        {
            if (((S2CLogin)message).Result == 1)
                isLogin = true;
        });
        NetManager.Connect(IP,Port);
        returnMainMenu.onClick.AddListener(() =>
        {
            if (isLogin)
                uIPanelMnr.TransactionPanel(PanelType.PanelMainMenu);
        });
    }

    private void Update()
    {
        NetManager.Update();
    }

    private void OnApplicationQuit()
    {
        NetManager.Close();
    }

}
