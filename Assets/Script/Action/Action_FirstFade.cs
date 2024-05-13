using Febucci.UI.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Action_FirstFade : Action_Command
{
    [SerializeField] private List<string> strings = new List<string>();

    public override void MouseEvent(MouseStatus mouseStatus)
    {
        switch (mouseStatus)
        {
            case MouseStatus.Excute:
                command.onMouseEvent -= MouseEvent;
                FadeManager.instance.FadeInImmediately(strings);
                break;
        }
    }


}