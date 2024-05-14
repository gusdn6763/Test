using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Action_Advanture : Action_Command
{
    public override void MouseEvent(MouseStatus mouseStatus)
    {
        switch (mouseStatus)
        {
            case MouseStatus.Excute:
                Player.instance.IsAdvanture = true;
                Destroy(gameObject);
                break;
        }
    }
}
