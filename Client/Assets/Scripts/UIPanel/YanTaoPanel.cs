using Google.Protobuf;
using Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YanTaoPanel : BasePanel<YanTao>
{
    protected override void InitRequese()
    {
        C2SYanTao c2SYanTao = new C2SYanTao();
        request = ProtobufParser.Encode(ProtoID.C2SYanTao, c2SYanTao);
        NetManager.AddMessageHander(ProtoID.S2CYanTao, ResponseHandler);
    }

    protected override void OnDisable()
    {
        if (dataDeleteList.Count != 0)
        {
            C2SDeleteYanTao c2SDeleteYanTao = new C2SDeleteYanTao();
            for (int i = 0; i < dataDeleteList.Count; ++i)
            {
                c2SDeleteYanTao.Name.Add(dataDeleteList[i]);
            }
            byte[] data = ProtobufParser.Encode(ProtoID.C2SDeleteYanTao, c2SDeleteYanTao);
            NetManager.Send(data);
            dataDeleteList.Clear();
        }
        if (dataAddDict.Count!=0)
        {
            C2SAddYanTao c2SAddYan = new C2SAddYanTao();
            foreach (var item in dataAddDict.Values)
            {
                c2SAddYan.YanTaoList.Add(item);
            }
            byte[] data = ProtobufParser.Encode(ProtoID.C2SAddYanTao, c2SAddYan);
            NetManager.Send(data);
            dataAddDict.Clear();
        }
        
        if(updateDataDict.Count!=0)
        {
            C2SUpdateYanTao c2SUpdateYanTao = new C2SUpdateYanTao();
            foreach (var item in updateDataDict.Values)
            {
                c2SUpdateYanTao.YanTaoList.Add(new UpdateYanTao() { Yantao = item.data, OldName = item.oldPK });
            }
            byte[] data = ProtobufParser.Encode(ProtoID.C2SUpdateYanTao, c2SUpdateYanTao);
            NetManager.Send(data);
            updateDataDict.Clear();
        }
    }

    protected override List<YanTao> ResponseDecode(IMessage message)
    {
        List<YanTao> yanTaoList = new List<YanTao>();
        YanTao yanTao;
        S2CYanTao s2CYanTao = (S2CYanTao)message;
        for (int i = 0; i < s2CYanTao.YanTaoList.Count; ++i)
        {
            yanTao = s2CYanTao.YanTaoList[i];
            yanTaoList.Add(yanTao);
            dataDict[yanTao.Name] = yanTao;
        }
        return yanTaoList;
    }
}
