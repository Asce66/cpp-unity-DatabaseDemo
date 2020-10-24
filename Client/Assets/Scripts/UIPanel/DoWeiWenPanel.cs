using Google.Protobuf;
using Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoWeiWenPanel : BaseSearchPanel<DoWeiWen>
{
    protected override void Awake()
    {
        base.Awake();
        NetManager.AddMessageHander(ProtoID.S2CSearchDoWeiwen, SpawnChilds);
        C2SSearchDoWeiwen c2SSearchDoWeiwen = new C2SSearchDoWeiwen();
        request = ProtobufParser.Encode(ProtoID.C2SSearchDoWeiwen, c2SSearchDoWeiwen);
    }

    protected override List<DoWeiWen> HandleMessage(IMessage message)
    {
        List<DoWeiWen> doWeiWens = new List<DoWeiWen>();
        S2CSearchDoWeiwen s2CSearchDoWeiwen = (S2CSearchDoWeiwen)message;
        foreach (var item in s2CSearchDoWeiwen.DoWeiWenList)
        {
            doWeiWens.Add(item);
        }
        return doWeiWens;
    }
}
