using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AnimationData", menuName = "Data/AnimationData", order = 2)]
public class AnimationData : ScriptableObject
{
    [Header("생성 애니메이션")]
    [SerializeField] public List<AppearanceAnimationScriptible> appearanceAnimation;

    [Header("행동 애니메이션")]
    [SerializeField] public List<BehaviorAnimationScriptible> behaviorAnimation;

    [Header("사라짐 애니메이션")]
    [SerializeField] public List<DisAppearanceAnimationScriptible> disAppearanceAnimation;
}
