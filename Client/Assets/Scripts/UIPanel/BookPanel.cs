using Google.Protobuf;
using Protocol;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class BookPanel : BasePanel<Book>
{
    protected override void InitRequese()
    {
        C2SBook c2SBook = new C2SBook();
        request = ProtobufParser.Encode(ProtoID.C2SBook, c2SBook);
        NetManager.AddMessageHander(ProtoID.S2CBook, ResponseHandler);
    }

    protected override void OnDisable()
    {
        if (dataDeleteList.Count != 0)
        {
            C2SDeleteBooks c2SDeleteBooks = new C2SDeleteBooks();
            for (int i = 0; i < dataDeleteList.Count; ++i)
            {
                Debug.Log("删除" + dataDeleteList[i]);
                c2SDeleteBooks.BhList.Add(dataDeleteList[i]);
            }
            byte[] data = ProtobufParser.Encode(ProtoID.C2SDeleteBooks, c2SDeleteBooks);
            NetManager.Send(data);
            dataDeleteList.Clear();
        }
        if (updateDataDict.Count != 0)
        {
            C2SUpdateBooks c2SUpdateBooks = new C2SUpdateBooks();
            foreach (var item in updateDataDict.Values)
            {
                c2SUpdateBooks.BookList.Add(new UpdateBook() { Book = item.data, OldBH = item.oldPK });
            }
            byte[] data = ProtobufParser.Encode(ProtoID.C2SUpdateBooks, c2SUpdateBooks);
            NetManager.Send(data);
            updateDataDict.Clear();
        }
        if (dataAddDict.Count!=0)
        {
            C2SAddBooks c2SAddBooks = new C2SAddBooks();
            foreach (var item in dataAddDict.Values)
            {
                c2SAddBooks.BookList.Add(item);
            }
            byte[] data = ProtobufParser.Encode(ProtoID.C2SAddBooks, c2SAddBooks);
            NetManager.Send(data);
            dataAddDict.Clear();            
        }      
    }

    protected override List<Book> ResponseDecode(IMessage message)
    {
        S2CBook s2CBook = (S2CBook)message;
        List<Book> bookList = new List<Book>();
        Book book = new Book();
        for(int i=0;i<s2CBook.BookList.Count;++i)
        {
            book = s2CBook.BookList[i];
            bookList.Add(book);
            dataDict[book.BH] = book;
        }
        return bookList;
    }
}
