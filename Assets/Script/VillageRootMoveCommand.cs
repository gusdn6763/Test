using Febucci.UI.Core;
using System.Collections;
using UnityEngine;

public class VillageRootMoveCommand : VillageMoveCommand, IMoveable
{
    #region ��ȣ�ۿ�
    public override bool IsInitCircleEnabled { set { initCircle.enabled = value; } }
    public override void Interaction(MouseStatus mouseStatus)
    {
        CurrentMouseStatus = mouseStatus;
        switch (mouseStatus)
        {
            case MouseStatus.Enter:
                rigi.velocity = Vector3.zero;
                AnimationManager.instance.InitialAnimation(ChildCommands);
                break;
            case MouseStatus.Down:
                AnimationManager.instance.Animation(ChildCommands, false);
                break;
            case MouseStatus.Up:
                AnimationManager.instance.InitialAnimation(ChildCommands);
                break;
            case MouseStatus.Drag:
                transform.position = GetMouseWorldPosition();
                break;
            case MouseStatus.Excute:
                AnimationManager.instance.Animation(ChildCommands, false);
                Excute();
                break;
            case MouseStatus.Exit:
                AnimationManager.instance.Animation(ChildCommands, false);
                break;
        }
        onMouseEvent?.Invoke(mouseStatus);
    }
    #endregion
    #region �ִϸ��̼�
    #endregion

    #region Moveable
    public Vector3 GetMouseWorldPosition()
    {
        Vector3 mousePoint = Input.mousePosition;
        mousePoint.z = Camera.main.WorldToScreenPoint(transform.position).z;

        return Camera.main.ScreenToWorldPoint(mousePoint);
    }
    public MouseStatus CurrentMouseStatus { get; set; }
    public void SetPower(Vector3 power)
    {
        rigi.velocity = power;
    }
    #endregion
}
