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
                float y = Mathf.Sin(t * Mathf.PI) * amplitude * character.uniformIntensity;

                for (int i = 0; i < character.current.positions.Length; i++)
                {
                    character.current.positions[i] = Vector3.Lerp(character.init.positions[i], character.source.positions[i] + Vector3.up * y, t);
                }
            }
            else
            {
                character.ResetPositions();
            }
        }
    }

}