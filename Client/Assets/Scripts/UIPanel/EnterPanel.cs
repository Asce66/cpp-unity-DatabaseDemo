using Google.Protobuf;
using Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 管理用户登录、注册的面板
/// </summary>
public class EnterPanel : MonoBehaviour
{
    InputField accountField, passwordField;

    Button loginBtn, registerBtn;

    Text tipsText;

    ReferenceCollector referenceCollector;
    IEnumerator enumerator = null;

    private void Start()
    {
        referenceCollector = GetComponentInChildren<ReferenceCollector>();
        accountField = referenceCollector.Get<GameObject>("Account").GetComponent<InputField>();
        passwordField = referenceCollector.Get<GameObject>("Password").GetComponent<InputField>();
        loginBtn = referenceCollector.Get<GameObject>("Btn_Login").GetComponent<Button>();
        registerBtn = referenceCollector.Get<GameObject>("Btn_Register").GetComponent<Button>();
        tipsText = referenceCollector.Get<GameObject>("Tips").GetComponent<Text>();
        loginBtn.onClick.AddListener(OnLoginBtnClick);
        registerBtn.onClick.AddListener(OnRegisterBtnClick);
        Color color = tipsText.color;
        color.a = 0;
        tipsText.color = color;

        NetManager.AddMessageHander(ProtoID.S2CLogin, S2CLoginHandler);
        NetManager.AddMessageHander(ProtoID.S2CRegister, S2CRegisterHandler);
    }


    void S2CRegisterHandler(IMessage message)
    {
        S2CRegister s2CRegister = (S2CRegister)message;
        if (s2CRegister.Result == 1)
        {
            if (enumerator != null)
                StopCoroutine(enumerator);
            StartCoroutine(enumerator = ShowTips("注册成功，请点击登录按钮进入管理系统！！", Color.green));

        }
        else
        {
            if (enumerator != null)
                StopCoroutine(enumerator);
            StartCoroutine(enumerator = ShowTips("注册失败，用户名已经存在！！", Color.red));
        }
    }

    void S2CLoginHandler(IMessage message)
    {
        S2CLogin s2CLogin = (S2CLogin)message;
        if (s2CLogin.Result == 1)
        {
            UIPanelMnr._Instance.TransactionPanel(PanelType.PanelMainMenu);
        }
        else
        {
            if (enumerator != null)
                StopCoroutine(enumerator);
            StartCoroutine(enumerator = ShowTips("用户名或者密码错误", Color.red, 3.0f));
        }
    }


    IEnumerator ShowTips(string content, Color color, float durating = 3.0f)
    {
        tipsText.gameObject.SetActive(true);
        float time = durating;
        tipsText.text = content;
        tipsText.color = color;
        while (durating > 0)
        {
            durating -= Time.deltaTime;
            color.a = durating / time;
            tipsText.color = color;
            yield return null;
        }
        enumerator = null;
    }

    bool CheckAccountPwd(string account, string password)
    {
        if (string.IsNullOrEmpty(account))
        {
            if (enumerator != null)
                StopCoroutine(enumerator);
            StartCoroutine(enumerator = ShowTips("用户名不能为空", Color.red, 3.0f));
            return false;
        }

        if (string.IsNullOrEmpty(password))
        {
            if (enumerator != null)
                StopCoroutine(enumerator);

            StartCoroutine(enumerator = ShowTips("密码不能为空", Color.red, 3.0f));
            return false;
        }
        return true;
    }

    void OnLoginBtnClick()
    {
        string account = accountField.text;
        string password = passwordField.text;
        if (CheckAccountPwd(account, password))
        {
            C2SLogin c2SLogin = new C2SLogin() { Account = account, Password = password };
            byte[] bytes = ProtobufParser.Encode(ProtoID.C2SLogin, c2SLogin);
            NetManager.Send(bytes);
        }
    }

    void OnRegisterBtnClick()
    {
        string account = accountField.text;
        string password = passwordField.text;
        if (CheckAccountPwd(account, password))
        {
            C2SRegister c2SRegister = new C2SRegister() { Account = account, Password = password };
            NetManager.Send(ProtobufParser.Encode(ProtoID.C2SRegister, c2SRegister));
        }
    }
}
