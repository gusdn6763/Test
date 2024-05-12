using System.Collections.Generic;
using UnityEngine;

public enum AppearanceAnimationType
{
    First,
    Default,
}

[System.Serializable]
public struct AppearanceAnimationScriptible
{
    public string[] animationString;

    [Header("글자별 애니메이션 대기시간")]
    public float waitForNormalChars;

    [Header("애니메이션 타입")]
    public AppearanceAnimationType appearanceAnimationType;
}

public enum BehaviorAnimationType
{
    Default,
    Enter,
    Clicking,
    Up
}

[System.Serializable]
public struct BehaviorAnimationScriptible
{
    public string[] animationString;

    [Header("애니메이션 타입")]
    public BehaviorAnimationType behaviorAnimationType;
}

public enum DisAppearanceAnimationType
{
    Default,
}
[System.Serializable]
public struct DisAppearanceAnimationScriptible
{
    public string[] animationString;

    [Header("글자별 애니메이션 대기시간")]
    public float waitForNormalChars;

    [Header("애니메이션 타입")]
    public DisAppearanceAnimationType disAppearanceAnimationType;
}

[System.Serializable]
public struct RevertScriptible
{
    public float revertTime;
}