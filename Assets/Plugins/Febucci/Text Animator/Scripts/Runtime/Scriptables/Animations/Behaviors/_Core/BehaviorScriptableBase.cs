using Febucci.UI.Core;

namespace Febucci.UI.Effects
{
    /// <summary>
    /// Base class for animating letters in Text Animator
    /// </summary>
    public abstract class BehaviorScriptableBase : AnimationScriptableBase
    {
        public override bool CanApplyEffectTo(CharacterData character, TAnimCore animator) => true;

        public override bool IsBehaviorDuration() 
        {
            if (duration > 0)
                return true;

            return false; 
        }
    }

}