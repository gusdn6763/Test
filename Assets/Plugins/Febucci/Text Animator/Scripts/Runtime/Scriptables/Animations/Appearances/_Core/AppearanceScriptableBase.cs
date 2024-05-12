using Febucci.UI.Core;
using UnityEngine;

namespace Febucci.UI.Effects
{
    /// <summary>
    /// Base class for animating letters in Text Animator
    /// </summary>
    [System.Serializable]
    public abstract class AppearanceScriptableBase : AnimationScriptableBase
    {
        public override bool CanApplyEffectTo(CharacterData character, TAnimCore animator) => character.passedTime <= duration;

        public override bool CanApplyEffectTo(MyCharacterData character, RectTransform rect) => character.appearanceTime <= duration;
    }

}