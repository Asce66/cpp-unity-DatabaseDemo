using Google.Protobuf;
using Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeiWenPanel : BasePanel<WeiWen>
{
    protected override void InitRequese()
    {
        C2SWeiWen c2SWeiWen = new C2SWeiWen();
        request = ProtobufParser.Encode(ProtoID.C2SWeiWen, c2SWeiWen);
        NetManager.AddMessageHander(ProtoID.S2CWeiWen, ResponseHandler);
    }

    protected override void OnDisable()
    {
        if (dataDeleteList.Count != 0)
        {
            C2SDeleteWeiWen c2SDeleteWeiWen = new C2SDeleteWeiWen();
            for (int i = 0; i < dataDeleteList.Count; ++i)
            {
                c2SDeleteWeiWen.Name.Add(dataDeleteList[i]);
            }
            byte[] data = ProtobufParser.Encode(ProtoID.C2SDeleteWeiWen, c2SDeleteWeiWen);
            NetManager.Send(data);
            dataDeleteList.Clear();
        }
        if (dataAddDict.Count != 0)
        {
            C2SAddWeiWen c2SAddWeiWen = new C2SAddWeiWen();
            foreach (var item in dataAddDict.Values)
            {
                c2SAddWeiWen.WeiWenList.Add(item);
            }
            byte[] data = ProtobufParser.Encode(ProtoID.C2SAddWeiWen, c2SAddWeiWen);
            NetManager.Send(data);
            dataAddDict.Clear();
        }
       
        if (updateDataDict.Count != 0)
        {
            C2SUpdateWeiWen c2SUpdateWeiWen = new C2SUpdateWeiWen();
            foreach (var item in updateDataDict.Values)
            {
                c2SUpdateWeiWen.WeiWenList.Add(new UpdateWeiWen() { Weiwen = item.data, OldName = item.oldPK });
            }
            byte[] data = ProtobufParser.Encode(ProtoID.C2SUpdateWeiWen, c2SUpdateWeiWen);
            NetManager.Send(data);
            updateDataDict.Clear();
        }
    }

    protected override List<WeiWen> ResponseDecode(IMessage message)
    {
        List<WeiWen> weiWens = new List<WeiWen>();
        S2CWeiWen s2CWeiWen = (S2CWeiWen)message;
        for (int i = 0; i < s2CWeiWen.WeiWenList.Count; ++i)
        {
            WeiWen weiWen = s2CWeiWen.WeiWenList[i];
            weiWens.Add(weiWen);
            dataDict[weiWen.Name] = weiWen;
        }
        return weiWens;
    }


}
