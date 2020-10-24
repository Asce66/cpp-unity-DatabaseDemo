using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;

public class CtrlHuman : BaseHuman
{
    protected override void Update()
    {
        base.Update();
        //移动
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 pos = Input.mousePosition;
            Ray ray = Camera.main.ScreenPointToRay(pos);
           
            RaycastHit hit;
            if(Physics.Raycast(ray,out hit))
            {
                if(hit.collider.gameObject.CompareTag("Terrain"))
                {
                    float x = hit.point.x, y = hit.point.y, z = hit.point.z;
                    StringBuilder sb = new StringBuilder("Move|");
                    sb.Append(desc + ",");
                    sb.Append(x + ",");
                    sb.Append(y + ",");
                    sb.Append(z);
                    TestNetManager.Send(sb.ToString());
                }
            }
        }

        if(Input.GetMouseButtonDown(1))
        {
            if (isAttack || isRun) return;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            Physics.Raycast(ray, out hit);
            transform.LookAt(hit.point);
            Attack();

            StringBuilder sb = new StringBuilder("Attack|");
            sb.Append(desc + ",");
            sb.Append(transform.eulerAngles.y);
            TestNetManager.Send(sb.ToString());

            Vector3 lineEnd = transform.position + 0.5f * transform.up;
            Vector3 lineStart = transform.position + 20 * transform.forward;
            RaycastHit hitCheck;
           if( Physics.Linecast(lineStart,lineEnd,out hitCheck))
            {
                if (hitCheck.collider.gameObject == gameObject) return;
                SyncHuman syncHuman = hitCheck.collider.GetComponent<SyncHuman>();
                if (syncHuman == null) return;
                StringBuilder sbb = new StringBuilder("Hit|");
                sbb.Append(syncHuman.desc + ",");
                sbb.Append(20.ToString());
                TestNetManager.Send(sbb.ToString());
            }
        }
    }
}
