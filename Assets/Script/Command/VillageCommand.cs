using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class VillageCommand : MultiTreeCommand
{
    #region ��ȣ�ۿ�

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

    #endregion
}