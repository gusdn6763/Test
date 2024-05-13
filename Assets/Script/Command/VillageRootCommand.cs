using Febucci.UI.Core;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public interface IMoveable
{
    void SetPower(Vector3 power);
    Vector3 GetMouseWorldPosition();
}

public class VillageRootCommand : VillageCommand, IMoveable
{
    public override bool IsInitCircleEnabled { set { initCircle.enabled = value; } }
    protected override void Awake()
    {
        initCircle = GetComponentInChildren<SpriteRenderer>(true);
        base.Awake();
        box.isTrigger = false;
    }

    #region 애니메이션
    public override BehaviorAnimationScriptible GetBehaviorTags()
    {
        return GetAnimationTags(animationData.behaviorAnimation, animation =>
        {
            switch (currentMouseStatus)
            {
                case MouseStatus.Enter:
                    return animation.behaviorAnimationType == BehaviorAnimationType.Enter;
                case MouseStatus.Down:
                    return animation.behaviorAnimationType == BehaviorAnimationType.Clicking;
                case MouseStatus.Up:
                    return animation.behaviorAnimationType == BehaviorAnimationType.Up;
                default:
                    return animation.behaviorAnimationType == BehaviorAnimationType.Default;
            }
        });
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
        onMouseEvent?.Invoke(mouseStatus);
    }
    #endregion

    #region 상호작용
    public override void Interaction(MouseStatus mouseStatus)
    {
        currentMouseStatus = mouseStatus;
        StartCoroutine(AnimaionCoroutine(mouseStatus));
    }
    #endregion

    #region Moveable
    public Vector3 GetMouseWorldPosition()
    {
        Vector3 mousePoint = Input.mousePosition;
        mousePoint.z = Camera.main.WorldToScreenPoint(transform.position).z;

        return Camera.main.ScreenToWorldPoint(mousePoint);
    }
    public void SetPower(Vector3 power)
    {
        rigi.velocity = power;
    }
    #endregion
}