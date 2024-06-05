using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationDataController : MonoBehaviour
{
    [SerializeField] protected AnimationData animationData;

    protected MultiTreeCommand command;

    protected virtual void Awake()
    {
        command = GetComponentInParent<MultiTreeCommand>();
    }

    public virtual AppearanceAnimationScriptible GetAppearanceTags()
    {
        foreach (AppearanceAnimationScriptible animation in animationData.appearanceAnimation)
        {
            if (animation.conditionType == AnimationConditionType.Default)
                return animation;
        }
        return AppearanceAnimationScriptible.GetDefault();
    }

    public virtual BehaviorAnimationScriptible GetBehaviorTags()
    {
        foreach (BehaviorAnimationScriptible animation in animationData.behaviorAnimation)
        {
            if (animation.conditionType == AnimationConditionType.Default)
                return animation;
        }
        return BehaviorAnimationScriptible.GetDefault();
    }

    public virtual DisAppearanceAnimationScriptible GetDisAppearanceTags()
    {
        foreach (DisAppearanceAnimationScriptible animation in animationData.disAppearanceAnimation)
        {
            if (animation.conditionType == AnimationConditionType.Default)
                return animation;
        }
        return DisAppearanceAnimationScriptible.GetDefault();
    }
}
