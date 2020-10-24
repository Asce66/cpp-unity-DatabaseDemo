using Google.Protobuf;
using Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WriteBookPanel : BaseSearchPanel<WriteBook>
{
    protected override void Awake()
    {
        base.Awake();
        NetManager.AddMessageHander(ProtoID.S2CSearchWriteBook, SpawnChilds);
        C2SSearchWriteBook c2SSearchWriteBook = new C2SSearchWriteBook();
        request = ProtobufParser.Encode(ProtoID.C2SSearchWriteBook, c2SSearchWriteBook);
    }

    protected override List<WriteBook> HandleMessage(IMessage message)
    {
        List<WriteBook> doWeiWens = new List<WriteBook>();
        S2CSearchWriteBook s2CSearchWriteBook = (S2CSearchWriteBook)message;
        foreach (var item in s2CSearchWriteBook.WriteBookList)
        {
            doWeiWens.Add(item);
        }
        return doWeiWens;
    }
}
