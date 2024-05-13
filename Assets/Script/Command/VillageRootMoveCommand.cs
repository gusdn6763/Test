using Febucci.UI.Core;
using System.Collections;
using UnityEngine;

public class VillageRootMoveCommand : VillageMoveCommand, IMoveable
{
    #region 상호작용
    public override bool IsInitCircleEnabled { set { initCircle.enabled = value; } }
    public override void Interaction(MouseStatus mouseStatus)
    {
        CurrentMouseStatus = mouseStatus;

        StartCoroutine(AnimaionCoroutine(mouseStatus));
        onMouseEvent?.Invoke(mouseStatus);
    }

    IEnumerator AnimaionCoroutine(MouseStatus mouseStatus)
    {
        switch (mouseStatus)
        {
            case MouseStatus.Enter:
                rigi.velocity = Vector3.zero;
                yield return StartCoroutine(AnimationManager.instance.IsBehaviorAnimationCoroutine(this));
                yield return StartCoroutine(AnimationManager.instance.InitialAnimationCoroutine(ChildCommands));
                break;
            case MouseStatus.Down:
                yield return StartCoroutine(AnimationManager.instance.AnimationCoroutine(ChildCommands, false));
                yield return StartCoroutine(AnimationManager.instance.SeparatorCoroutine(this, true));
                break;
            case MouseStatus.Up:
                yield return StartCoroutine(AnimationManager.instance.SeparatorCoroutine(this, false));
                break;
            case MouseStatus.Drag:
                transform.position = GetMouseWorldPosition();
                break;
            case MouseStatus.Exit:
                yield return StartCoroutine(AnimationManager.instance.AnimationCoroutine(ChildCommands, false));
                yield return StartCoroutine(AnimationManager.instance.IsBehaviorAnimationCoroutine(this));
                break;
        }
    }
    #endregion
    #region 애니메이션
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
