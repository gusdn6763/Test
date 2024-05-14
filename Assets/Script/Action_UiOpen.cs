using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Action_UiOpen : Action_Command
{
    [SerializeField] UIScript uIScript;

    public override void MouseEvent(MouseStatus mouseStatus)
    {
        if (mouseStatus == MouseStatus.Excute)
        {
            uIScript.OpenClose(true);
        }        
    }
}
