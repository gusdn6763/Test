using Febucci.UI.Core;
using UnityEngine;

namespace Febucci.UI.Effects
{
    [UnityEngine.Scripting.Preserve]
    [CreateAssetMenu(fileName = "Scale Appearance", menuName = "Text Animator/Animations/Appearances/Scale")]
    [EffectInfo("Scale", EffectCategory.Appearances)]
    public sealed class ScaleAppearance : AppearanceScriptableBase
    {
        public TimeMode timeMode = new TimeMode(true);
        [EmissionCurveProperty] public EmissionCurve emissionCurve = new EmissionCurve();

        public override void ApplyEffectTo(ref Core.CharacterData character, TAnimCore animator)
        {
        }

        public override void ApplyEffectTo(ref MyCharacterData character, RectTransform rect)
        {
            float timePassed = timeMode.GetTime(character.appearanceTime, character.appearanceTime, character.index);

            if (timePassed < 0)
                return;

            float time = Tween.EaseInOut(character.appearanceTime / duration);

            if (time < 1)
            {
                float weight = emissionCurve.Evaluate(time);
                float round = Mathf.Floor(weight * 100f) / 100f;
                rect.localScale = new Vector3(round, round, round);
            }
            else
                rect.localScale = Vector3.one;
        }
    }
}