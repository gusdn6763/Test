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

    [Header("���ں� �ִϸ��̼� ���ð�")]
    public float waitForNormalChars;

    [Header("�ִϸ��̼� Ÿ��")]
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

    [Header("�ִϸ��̼� Ÿ��")]
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

    [Header("���ں� �ִϸ��̼� ���ð�")]
    public float waitForNormalChars;

    [Header("�ִϸ��̼� Ÿ��")]
    public DisAppearanceAnimationType disAppearanceAnimationType;
}

[System.Serializable]
public struct RevertScriptible
{
    public float revertTime;
}