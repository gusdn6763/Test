using Febucci.UI.Core;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public interface IRootCommand
{
    void SetPower(Vector3 power);
}

public class VillageRootCommand : VillageCommand, IRootCommand
{
    #region 애니메이션
    protected MouseStatus currentMouseStatus;
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
    #endregion

    #region 상호작용
    public override void Interaction(MouseStatus mouseStatus)
    {
        currentMouseStatus = mouseStatus;
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