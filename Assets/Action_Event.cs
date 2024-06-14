using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Action_Event : Action_Command
{
    public UnityEvent test;
    public override void AnimationEvent(MouseStatus mouseStatus) 
    {
        if (mouseStatus == MouseStatus.Excute)
            test?.Invoke();
    }
}
