using Google.Protobuf;
using Google.Protobuf.Collections;
using Protocol;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum DropDownType
{
    WriteBooks,
    WeiWen,
    YanTao
}

public class WriterPanel : MonoBehaviour
{

    public class DropDownManager
    {
        public Dictionary<int, Action<string>> sendMessageHandler = new Dictionary<int, Action<string>>();

        DropDownType downType;
        public Dropdown dropdown;
        private Text label;

        public DropDownManager(Dropdown dropdown)
        {
            this.dropdown = dropdown;
            label = dropdown.transform.Find("Label").GetComponent<Text>();
            dropdown.gameObject.SetActive(false);
            dropdown.onValueChanged.AddListener(FinishedSelect);
        }

        public bool ShowDropDown(List<string> chooses, Vector3 position, DropDownType dropDownType)
        {
            this.downType = dropDownType;
            dropdown.gameObject.SetActive(true);
            dropdown.transform.position = position;

            if (dropdown.options.Count <= 0)
                dropdown.options.Add(new Dropdown.OptionData() { text = "" });
            else
                dropdown.options[0].text = "";
            int i = 1;
            for (; i < dropdown.options.Count && i <= chooses.Count; ++i)
            {
                dropdown.options[i].text = chooses[i - 1];
            }
            if (i > chooses.Count)
            {
                dropdown.options.RemoveRange(i, dropdown.options.Count - chooses.Count - 1);
            }
            else
            {
                for (; i <= chooses.Count; ++i)
                {
                    dropdown.options.Add(new Dropdown.OptionData() { text = chooses[i - 1] });
                }
            }
            label.text = "";
            dropdown.SetValueWithoutNotify(0);
            return true;
        }

        void FinishedSelect(int ind)
        {
            if (sendMessageHandler.ContainsKey((int)downType) == false)
            {
                Debug.LogError("没有处理下拉框发送数据的情况：" + downType.ToString());
                return;
            }
            sendMessageHandler[(int)downType](dropdown.options[ind].text);
            UIPanelMnr._Instance.ShowPrompt("添加成功!!!");
            dropdown.gameObject.SetActive(false);
        }
    }

    byte[] c2SWriterData;
    GameObject writerPrefab;
    Transform content;

    private Dictionary<string, Writer> writerDict = new Dictionary<string, Writer>();//现在存放的作者数据(添加新作者的时候编号查重用)
    private Dictionary<string, Writer> addWriterDict = new Dictionary<string, Writer>();//想要新增加的列表
    private List<string> deleteWSriterList = new List<string>();//想要替换的列表
    private Dictionary<string, UpdateWriter> updateWriterDict = new Dictionary<string, UpdateWriter>();

    private float oneItemHeight;//一个元素所占据的视图高度

    private DropDownManager dropDownManager;
    private string dropDownWriterBH;//下拉框作用于的作者编号，发送至服务器更新数据库
    private DropDownType dropDownType;
    private Vector3 dropDownPosition;

    private List<DoYanTao> doYanTaos = new List<DoYanTao>();//新增的研讨记录
    private List<DoWeiWen> doWeiWens = new List<DoWeiWen>();
    private List<WriteBook> writeBooks = new List<WriteBook>();

    private void Awake()
    {
        Protocol.C2SWriter c2SWriter = new Protocol.C2SWriter();
        c2SWriterData = ProtobufParser.Encode(ProtoID.C2SWriter, c2SWriter);

        ReferenceCollector referenceCollector = GetComponent<ReferenceCollector>();
        writerPrefab = referenceCollector.Get<GameObject>("Writer");
        content = referenceCollector.Get<GameObject>("Content").transform;
        GridLayoutGroup gridLayoutGroup = content.GetComponent<GridLayoutGroup>();
        oneItemHeight = gridLayoutGroup.cellSize.y + gridLayoutGroup.spacing.y;
        referenceCollector.Get<GameObject>("BtnNew").GetComponent<Button>().onClick.AddListener(() => NewItem(true));

        NetManager.AddMessageHander(ProtoID.S2CWriter, S2CWriterHandler);
        Dropdown dropdown = referenceCollector.Get<GameObject>("Dropdown").GetComponent<Dropdown>();
        dropDownManager = new DropDownManager(dropdown);
        InitDropDownHandler();
    }

    /// <summary>
    /// 创建一个新的元素
    /// </summary>
    private void NewItem(bool canEdit)
    {
        Vector2 size = content.GetComponent<RectTransform>().sizeDelta;
        size.y += oneItemHeight;
        content.GetComponent<RectTransform>().sizeDelta = size;

        GameObject writer = Instantiate(writerPrefab, content);
        WriterItem writerItem = writer.GetComponent<WriterItem>();
        writerItem.writerPanel = this;
        writerItem.isEditor = true;
        writerItem.CheckBtnSwitch(canEdit);
    }

    private void OnEnable()
    {
        NetManager.Send(c2SWriterData);
    }

    private void OnDisable()
    {
        dropDownManager.dropdown.gameObject.SetActive(false);
        //先删除再增加，这个顺序不能换 如果有一个元素是新增的，但是又被删除了，后面又想增加同主键元素
        //假如是先增加后删除的顺序发消息，数据库那边就执行增加操作，后执行删除操作，结果导致前后作用抵消，数据没有记录
        if (deleteWSriterList.Count != 0)
        {
            C2SDeleteWriters c2SDeleteWriters = new C2SDeleteWriters();
            foreach (var item in deleteWSriterList)
            {
                c2SDeleteWriters.WriterList.Add(item);
            }
            byte[] data = ProtobufParser.Encode(ProtoID.C2SDeleteWriters, c2SDeleteWriters);
            NetManager.Send(data);
            deleteWSriterList.Clear();
        }
        if (addWriterDict.Count != 0)
        {
            C2SAddWriters c2SAddWriters = new C2SAddWriters();
            foreach (var item in addWriterDict.Values)
            {
                c2SAddWriters.WriterList.Add(item);
            }
            byte[] data = ProtobufParser.Encode(ProtoID.C2SAddWriters, c2SAddWriters);
            NetManager.Send(data);
            addWriterDict.Clear();
        }
        
        if (updateWriterDict.Count != 0)
        {
            C2SUpdateWriters c2SUpdateWriters = new C2SUpdateWriters();
            foreach (var item in updateWriterDict.Values)
            {
                c2SUpdateWriters.WriterList.Add(item);
            }
            byte[] data = ProtobufParser.Encode(ProtoID.C2SUpdateWriters, c2SUpdateWriters);
            NetManager.Send(data);
            updateWriterDict.Clear();
        }
        if (doYanTaos.Count != 0)
        {
            C2SAddDoYanTao c2SAddDoYanTao = new C2SAddDoYanTao();
            foreach (var item in doYanTaos)
            {
                c2SAddDoYanTao.DoYanTaoList.Add(item);
            }
            byte[] data = ProtobufParser.Encode(ProtoID.C2SAddDoYanTao, c2SAddDoYanTao);
            NetManager.Send(data);
            doYanTaos.Clear();
        }
        if (doWeiWens.Count != 0)
        {
            C2SAddDoWeiWen c2SAddDoWeiWen = new C2SAddDoWeiWen();
            foreach (var item in doWeiWens)
            {
                c2SAddDoWeiWen.DoWeiWenList.Add(item);
            }
            byte[] data = ProtobufParser.Encode(ProtoID.C2SAddDoWeiWen, c2SAddDoWeiWen);
            NetManager.Send(data);
            doWeiWens.Clear();
        }
        if (writeBooks.Count != 0)
        {
            C2SAddWriteBook c2SAddWriteBook = new C2SAddWriteBook();
            foreach (var item in writeBooks)
            {
                c2SAddWriteBook.WriteBookList.Add(item);
            }
            byte[] data = ProtobufParser.Encode(ProtoID.C2SAddWriteBook, c2SAddWriteBook);
            NetManager.Send(data);
            writeBooks.Clear();
        }

    }

    /// <summary>
    /// 增加元素到新加数组
    /// 后面统一把所有新加元素发送至服务端
    /// </summary>
    public bool AddToAddList(Writer writer)
    {
        if (writerDict.ContainsKey(writer.BH))
        {
            return false;
        }
        addWriterDict[writer.BH] = writer;
        writerDict[writer.BH] = writer;
        return true;
    }

    public bool UpdateWriter(Writer writer, string oldBH, bool needCheck)
    {
        if (needCheck && writerDict.ContainsKey(writer.BH))
            return false;
        writerDict.Remove(oldBH);
        writerDict[writer.BH] = writer;
        //如果更新的元素数据是新增列表里的，就直接更新新增列表对应数据
        //不再向修改列表增加数据
        if (addWriterDict.ContainsKey(oldBH))
        {
            addWriterDict.Remove(oldBH);
            addWriterDict[writer.BH] = writer;
        }
        else
        {
            if (updateWriterDict.ContainsKey(oldBH))
            {
                UpdateWriter updateWriter = updateWriterDict[oldBH];
                updateWriterDict.Remove(oldBH);
                updateWriter.Writer = writer;
                updateWriterDict[writer.BH] = updateWriter;
            }
            else
            {
                updateWriterDict[writer.BH] = new UpdateWriter() { Writer = writer, OldBH = oldBH };
            }
        }
        return true;

    }
    public void DeleteWriter(string writerBH)
    {
        //防止这样一种情况:新增一个元素但是元素的数据都还没保存
        if (writerDict.ContainsKey(writerBH))
        {
            writerDict.Remove(writerBH);
            deleteWSriterList.Add(writerBH);
        }
        if (addWriterDict.ContainsKey(writerBH))
            addWriterDict.Remove(writerBH);
        else if (updateWriterDict.ContainsKey(writerBH))
            updateWriterDict.Remove(writerBH);

    }

    /// <summary>
    /// 增加元素到删除数组
    /// 后面统一把所有需要删除的元素发送至服务端
    /// </summary>
    public void AddToDeleteList(string str)
    {
        deleteWSriterList.Add(str);
    }

    void S2CWriterHandler(IMessage message)
    {
        S2CWriter s2CWriter = (S2CWriter)message;
        foreach (var item in s2CWriter.WriterList)
        {
            writerDict[item.BH] = item;
        }
        float height = s2CWriter.WriterList.Count * oneItemHeight;
        Vector2 size = content.GetComponent<RectTransform>().sizeDelta;
        size.y = height;
        content.GetComponent<RectTransform>().sizeDelta = size;

        //根据服务端发来的数据生成元素物体
        int i = 0;
        for (; i < s2CWriter.WriterList.Count && i < content.childCount; ++i)
        {
            content.GetChild(i).gameObject.SetActive(true);
            WriterItem writerItem = content.GetChild(i).GetComponent<WriterItem>();
            writerItem.writerPanel = this;
            writerItem.UpdateWriterData(s2CWriter.WriterList[i]);
            writerItem.CheckBtnSwitch(false);
            writerItem.isFirst = false;
        }
        if (i >= s2CWriter.WriterList.Count)
        {
            for (; i < content.childCount; ++i)
            {
                content.GetChild(i).gameObject.SetActive(false);
            }
        }
        else
        {
            for (; i < s2CWriter.WriterList.Count; ++i)
            {
                GameObject go = Instantiate(writerPrefab, content);
                go.GetComponent<WriterItem>().writerPanel = this;
                WriterItem writerItem = content.GetChild(i).GetComponent<WriterItem>();
                writerItem.UpdateWriterData(s2CWriter.WriterList[i]);
                writerItem.CheckBtnSwitch(false);
                writerItem.isFirst = false;
            }
        }
    }

    /// <summary>
    /// 添加下拉框的监听函数(接收数据和发送数据成对监听)
    /// </summary>
    private void InitDropDownHandler()
    {
        NetManager.AddMessageHander(ProtoID.S2CWriteBook, (message) =>
        {
            S2CWriteBook s2CWriteBook = (S2CWriteBook)message;
            if (s2CWriteBook.WriteBookList.Count <= 0)
            {
                UIPanelMnr._Instance.ShowPrompt("没有保存书籍记录");
                return;
            }
            List<string> str = new List<string>();
            foreach (var item in s2CWriteBook.WriteBookList)
            {
                str.Add(item.BH);
            }
            dropDownManager.ShowDropDown(str, dropDownPosition, dropDownType);
        }
        );
        NetManager.AddMessageHander(ProtoID.S2cDoWeiWen, (message) =>
        {
            S2cDoWeiWen s2cDoWeiWen = (S2cDoWeiWen)message;
            if (s2cDoWeiWen.DoWeiWenList.Count <= 0)
            {
                UIPanelMnr._Instance.ShowPrompt("没有保存慰问记录");
                return;
            }
            List<string> str = new List<string>();
            foreach (var item in s2cDoWeiWen.DoWeiWenList)
            {
                str.Add(item.Name);
            }
            dropDownManager.ShowDropDown(str, dropDownPosition, dropDownType);
        }
       );
        NetManager.AddMessageHander(ProtoID.S2CDoYanTao, (message) =>
        {
            S2CDoYanTao s2CDoYanTao = (S2CDoYanTao)message;
            if (s2CDoYanTao.DoYanTaoList.Count <= 0)
            {
                UIPanelMnr._Instance.ShowPrompt("没有保存研讨记录");
                return;
            }
            List<string> str = new List<string>();
            foreach (var item in s2CDoYanTao.DoYanTaoList)
            {
                str.Add(item.Name);
            }
            dropDownManager.ShowDropDown(str, dropDownPosition, dropDownType);
        }
       );

        dropDownManager.sendMessageHandler[(int)DropDownType.WeiWen] = (weiwenName) =>
        {
            if (string.IsNullOrEmpty(weiwenName))
                return;
            doWeiWens.Add(new DoWeiWen() { Name = weiwenName, SBH = dropDownWriterBH });
        };
        dropDownManager.sendMessageHandler[(int)DropDownType.WriteBooks] = (bookName) =>
        {
            if (string.IsNullOrEmpty(bookName))
                return;
            writeBooks.Add(new WriteBook() { BBH = bookName, SBH = dropDownWriterBH });
        };
        dropDownManager.sendMessageHandler[(int)DropDownType.YanTao] = (yantaoName) =>
        {
            if (string.IsNullOrEmpty(yantaoName))
                return;
            doYanTaos.Add(new DoYanTao() { Name = yantaoName, SBH = dropDownWriterBH });
        };
    }

    public void ShowDropDown(string writerBH, DropDownType dropDownType, Vector3 position)
    {
        this.dropDownWriterBH = writerBH;
        this.dropDownType = dropDownType;
        dropDownPosition = position;
        byte[] data = null;
        switch (dropDownType)
        {
            case DropDownType.WriteBooks:
                data = ProtobufParser.Encode(ProtoID.C2SWriteBook, new C2SWriteBook());
                break;
            case DropDownType.WeiWen:
                data = ProtobufParser.Encode(ProtoID.C2SDoWeiWen, new C2SDoWeiWen());
                break;
            case DropDownType.YanTao:
                data = ProtobufParser.Encode(ProtoID.C2SDoYanTao, new C2SDoYanTao());
                break;
            default:
                break;
        }
        NetManager.Send(data);
    }

}
