using Febucci.UI.Core;
using UnityEngine;

namespace Febucci.UI.Effects
{
    [UnityEngine.Scripting.Preserve]
    [CreateAssetMenu(menuName = "Text Animator/Animations/Behaviors/Revert", fileName = "Revert Behavior")]
    [EffectInfo("revert", EffectCategory.Behaviors)]
    [DefaultValue(nameof(baseAmplitude), 7.27f)]
    [DefaultValue(nameof(baseFrequency), 4f)]
    [DefaultValue(nameof(baseWaveSize), .4f)]
    public sealed class RevertBehavior : BehaviorScriptableSine
    {
        public override void ApplyEffectTo(ref Core.CharacterData character, TAnimCore animator)
        {

        }

        public override void ApplyEffectToBehaviorDuration(ref MyCharacterData character, RectTransform rect)
        {
            if (character.passedTime <= duration)
            {
                float t = character.passedTime / duration;
                for (int i = 0; i < character.current.positions.Length; i++)
                {
                    character.current.positions[i] = Vector3.Lerp(character.currentBefore.positions[i], character.current.positions[i], t);
                }
            }
            else
            {
                character.SaveBeforePositions();
            }
        }
    }

}