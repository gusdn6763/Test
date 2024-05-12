using Febucci.UI.Core;

namespace Febucci.UI.Effects
{
    /// <summary>
    /// Base class for animating letters in Text Animator
    /// </summary>
    public abstract class BehaviorScriptableBase : AnimationScriptableBase
    {
        public override bool CanApplyEffectTo(CharacterData character, TAnimCore animator) => true;
    }

}