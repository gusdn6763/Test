using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sprite_OutLine : Action_Command
{
    private TextOutLine outLine;

    private void Start()
    {
        outLine = Instantiate(PrefabManager.instance.outLine, transform);
    }

    public override void MouseEvent(MouseStatus mouseStatus)
    {
        switch (mouseStatus)
        {
            case MouseStatus.Enter:
                outLine.gameObject.SetActive(true);
                break;
            case MouseStatus.Exit:
                outLine.gameObject.SetActive(false);
                break;
        }
    }
}
