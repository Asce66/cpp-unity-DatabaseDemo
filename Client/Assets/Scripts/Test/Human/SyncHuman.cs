using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SyncHuman : BaseHuman
{
   public void SynAttack(float eurly)
    {
        transform.eulerAngles = new Vector3(0, eurly, 0);
        Attack();
    }
}
