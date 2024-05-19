using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemCommand : MultiTreeCommand
{
    public bool IsAnimation { get; set; }

    public override void Interaction(MouseStatus mouseStatus)
    {
        switch (mouseStatus)
        {
            case MouseStatus.Enter:
                rigi.velocity = Vector3.zero;
                break;
        }

        base.Interaction(mouseStatus);
    }

    public void SetPower(Vector3 power)
    {
        rigi.velocity = power;
    }
}
