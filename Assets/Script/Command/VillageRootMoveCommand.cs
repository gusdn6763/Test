using Febucci.UI.Core;
using System.Collections;
using UnityEngine;

public class VillageRootMoveCommand : VillageMoveCommand, IRootCommand
{
    #region 상호작용
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

    #region Moveable

    public void SetPower(Vector3 power)
    {
        rigi.velocity = power;
    }
    #endregion
}
