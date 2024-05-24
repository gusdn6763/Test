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
            case MouseStatus.Down:
                outLine.gameObject.SetActive(true);
                break;
            case MouseStatus.Up:
                outLine.gameObject.SetActive(false);
                break;
        }
    }
}
