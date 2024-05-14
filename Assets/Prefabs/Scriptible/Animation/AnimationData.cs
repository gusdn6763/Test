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
}
