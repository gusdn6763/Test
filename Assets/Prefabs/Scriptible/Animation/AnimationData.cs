using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AnimationData", menuName = "Data/AnimationData", order = 2)]
public class AnimationData : ScriptableObject
{
    [Header("���� �ִϸ��̼�")]
    [SerializeField] public List<AppearanceAnimationScriptible> appearanceAnimation;

    [Header("�ൿ �ִϸ��̼�")]
    [SerializeField] public List<BehaviorAnimationScriptible> behaviorAnimation;

    [Header("����� �ִϸ��̼�")]
    [SerializeField] public List<DisAppearanceAnimationScriptible> disAppearanceAnimation;

    public AppearanceAnimationScriptible FindAppearanceType(AnimationConditionType animationConditionType)
    {
        for(int i = 0; i < appearanceAnimation.Count; i++)
        {
            if (animationConditionType == appearanceAnimation[i].conditionType)
                return appearanceAnimation[i];
        }

        return AppearanceAnimationScriptible.GetDefault();
    }

    public DisAppearanceAnimationScriptible FindDisAppearanceType(AnimationConditionType animationConditionType)
    {
        for (int i = 0; i < disAppearanceAnimation.Count; i++)
        {
            if (animationConditionType == disAppearanceAnimation[i].conditionType)
                return disAppearanceAnimation[i];
        }

        return DisAppearanceAnimationScriptible.GetDefault();
    }
}
