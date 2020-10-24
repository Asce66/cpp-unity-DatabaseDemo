using Google.Protobuf;
using Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoYanTaoPanel : BaseSearchPanel<DoYanTao>
{
    protected override void Awake()
    {
        base.Awake();
        NetManager.AddMessageHander(ProtoID.S2CSearchDoYanTao, SpawnChilds);
        C2SSearchDoYanTao c2SSearchDoYanTao = new C2SSearchDoYanTao();
        request = ProtobufParser.Encode(ProtoID.C2SSearchDoYanTao, c2SSearchDoYanTao);
    }

    protected override List<DoYanTao> HandleMessage(IMessage message)
    {
        List<DoYanTao> doWeiWens = new List<DoYanTao>();
        S2CSearchDoYanTao s2CSearchDoWeiwen = (S2CSearchDoYanTao)message;
        foreach (var item in s2CSearchDoWeiwen.DoYanTaoList)
        {
            doWeiWens.Add(item);
        }
        return doWeiWens;
    }
}
