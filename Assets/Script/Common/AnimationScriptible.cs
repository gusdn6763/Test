using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct AppearanceAnimationScriptible
{
    public string[] animationString;

    [Header("글자별 애니메이션 대기시간")]
    public float waitForNormalChars;

    [Header("애니메이션 타입")]
    public AnimationConditionType conditionType;

    public static AppearanceAnimationScriptible GetDefault()
    {
        return new AppearanceAnimationScriptible
        {
            animationString = null,
            waitForNormalChars = 0f,
            conditionType = AnimationConditionType.Default
        };
    }
}

[System.Serializable]
public struct BehaviorAnimationScriptible
{
    public string[] animationString;

    [Header("애니메이션 타입")]
    public AnimationConditionType conditionType;

    public bool isLoop;
    public static BehaviorAnimationScriptible GetDefault()
    {
        return new BehaviorAnimationScriptible
        {
            animationString = null,
            isLoop = false,
            conditionType = AnimationConditionType.Default
        };
    }
}

[System.Serializable]
public struct DisAppearanceAnimationScriptible
{
    public string[] animationString;

    [Header("글자별 애니메이션 대기시간")]
    public float waitForNormalChars;

    [Header("애니메이션 타입")]
    public AnimationConditionType conditionType;

    public static DisAppearanceAnimationScriptible GetDefault()
    {
        return new DisAppearanceAnimationScriptible
        {
            animationString = null,
            waitForNormalChars = 0f,
            conditionType = AnimationConditionType.Default
        };
    }
}