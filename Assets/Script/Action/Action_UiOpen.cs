using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct UiStatus
{
    public MouseStatus mouseStatus;
    public UIScript uIScript;
}

public class Action_UiOpen : Action_Command
{
    [SerializeField] UiStatus uiStatus;

    public override void AnimationEvent(MouseStatus mouseStatus)
    {
        if (uiStatus.mouseStatus == mouseStatus)
        {
            uiStatus.uIScript.OpenClose(true);
        }        
    }
}
