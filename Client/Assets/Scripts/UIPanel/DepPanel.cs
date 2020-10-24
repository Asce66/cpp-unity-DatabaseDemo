using Google.Protobuf;
using Protocol;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DepPanel : BasePanel<Department>
{
    protected override void InitRequese()
    {
        C2SDepartment c2SDepartment = new C2SDepartment();
        request = ProtobufParser.Encode(ProtoID.C2SDepartment, c2SDepartment);
        NetManager.AddMessageHander(ProtoID.S2CDepartment, ResponseHandler);
    }

    protected override void OnDisable()
    {
        if (dataDeleteList.Count != 0)
        {
            C2SDeleteDepartment c2SDeleteDepartment = new C2SDeleteDepartment();
            for (int i = 0; i < dataDeleteList.Count; ++i)
            {
                c2SDeleteDepartment.BhList.Add(dataDeleteList[i]);
            }
            byte[] data = ProtobufParser.Encode(ProtoID.C2SDeleteDepartment, c2SDeleteDepartment);
            NetManager.Send(data);
            dataDeleteList.Clear();
        }
        if (dataAddDict.Count!=0)
        {
            C2SAddDepartment c2SAddDepartment = new C2SAddDepartment();
            foreach (var item in dataAddDict.Values)
            {
                c2SAddDepartment.DepartmentList.Add(item);
            }
            byte[] data = ProtobufParser.Encode(ProtoID.C2SAddDepartment, c2SAddDepartment);
            NetManager.Send(data);
            dataAddDict.Clear();
        }
        if(updateDataDict.Count!=0)
        {
            C2SUpdateDepartment c2SUpdateDepartment = new C2SUpdateDepartment();
            foreach (var item in updateDataDict.Values)
            {
                c2SUpdateDepartment.DepartmentList.Add(new UpdateDepartment() { Department = item.data, OldBH = item.oldPK });
            }
            byte[] data = ProtobufParser.Encode(ProtoID.C2SUpdateDepartment, c2SUpdateDepartment);
            NetManager.Send(data);
            updateDataDict.Clear();
        }
       
    }

    protected override List<Department> ResponseDecode(IMessage message)
    {
        List<Department> departmentList = new List<Department>();
        S2CDepartment s2CDepartment = (S2CDepartment)message;
        for(int i=0;i<s2CDepartment.DepartmentList.Count;++i)
        {
            Department department = s2CDepartment.DepartmentList[i];
            departmentList.Add(department);
            dataDict[department.BMH] = department;
        }
        return departmentList;
    }
}
