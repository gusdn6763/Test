using Febucci.UI.Core;
using UnityEngine;

namespace Febucci.UI.Effects
{
    [UnityEngine.Scripting.Preserve]
    [CreateAssetMenu(menuName = "Text Animator/Animations/Behaviors/Wave", fileName = "Wave Behavior")]
    [EffectInfo("wave", EffectCategory.Behaviors)]
    [DefaultValue(nameof(baseAmplitude), 7.27f)]
    [DefaultValue(nameof(baseFrequency), 4f)]
    [DefaultValue(nameof(baseWaveSize), .4f)]
    public sealed class WaveBehavior : BehaviorScriptableSine
    {
        public float threshold = 0.1f; // 중앙 근접 임계값
        public override void ApplyEffectTo(ref Core.CharacterData character, TAnimCore animator)
        {
            character.current.positions.MoveChar(
                Vector3.up * Mathf.Sin(animator.time.timeSinceStart * frequency + character.index * waveSize) 
                * amplitude * character.uniformIntensity);
        }

        public override void ApplyEffectTo(ref MyCharacterData character, RectTransform rect) 
        {
            character.current.positions.MoveChar(
            Vector3.up * Mathf.Sin(character.passedTime * frequency + character.index * waveSize)
            * amplitude * character.uniformIntensity);
        }

        public override void ApplyEffectToBehaviorDuration(ref MyCharacterData character, RectTransform rect)
        {
            float t = (character.passedTime % duration) / duration;
            float y = Mathf.Sin(t * Mathf.PI * 2f + character.index * waveSize) * amplitude * character.uniformIntensity;
            character.current.positions.MoveChar(Vector3.up * y);
        }
    }

}