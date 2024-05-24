public class VillageAnimationDataController : AnimationDataController
{
    public override AppearanceAnimationScriptible GetAppearanceTags()
    {
        foreach (AppearanceAnimationScriptible animation in animationData.appearanceAnimation)
        {
            if (command.IsFirstAppearance)
            {
                if (animation.conditionType == AnimationConditionType.First)
                    return animation;
            }
            else
            {
                if (animation.conditionType == AnimationConditionType.Default)
                    return animation;
            }
        }
        return AppearanceAnimationScriptible.GetDefault();
    }

    public override BehaviorAnimationScriptible GetBehaviorTags()
    {
        foreach (BehaviorAnimationScriptible animation in animationData.behaviorAnimation)
        {
            switch (command.CurrentMouseStatus)
            {
                case MouseStatus.Enter:
                    if (animation.conditionType == AnimationConditionType.Enter)
                        return animation;
                    break;
                case MouseStatus.Down:
                    if (animation.conditionType == AnimationConditionType.Clicking)
                        return animation;
                    break;
                case MouseStatus.Up:
                    if (animation.conditionType == AnimationConditionType.Up)
                        return animation;
                    break;
                default:
                    if (animation.conditionType == AnimationConditionType.Default)
                        return animation;
                    break;
            }
        }
        return BehaviorAnimationScriptible.GetDefault();
    }

    public override DisAppearanceAnimationScriptible GetDisAppearanceTags()
    {
        foreach (DisAppearanceAnimationScriptible animation in animationData.disAppearanceAnimation)
        {
            if (animation.conditionType == AnimationConditionType.Default)
                return animation;
        }
        return DisAppearanceAnimationScriptible.GetDefault();
    }
}