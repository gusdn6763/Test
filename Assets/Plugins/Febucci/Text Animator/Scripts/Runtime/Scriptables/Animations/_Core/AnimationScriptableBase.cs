using Febucci.UI.Core;
using UnityEngine;

namespace Febucci.UI.Effects
{
    /// <summary>
    /// Base class for animating letters in Text Animator
    /// </summary>
    public abstract class AnimationScriptableBase : ScriptableObject, ITagProvider
    {
        public float baseDuration;
        protected float duration;

        [SerializeField] string tagID;
        public string TagID
        {
            get => tagID;
            set => tagID = value; //TODO if playing, discard rebuild if already initialized
        }

        //--- INITIALIZATION ---
        bool initialized = false;
        public void InitializeOnce()
        { 
            if(initialized) return;

            initialized = true;

            OnInitialize();
        }

        protected virtual void OnInitialize(){ }

        void OnEnable()
        {
            //resets for enter playmode settings
            initialized = false;
        }

        //--- ABSTRACT / VIRTUAL METHODS ---+

        /// <summary>
        /// Resets the effect context (base variables) for every region, before applying modifiers (if any) with <see cref="SetModifier"/>
        /// </summary>
        public virtual void ResetContext(TAnimCore animator)
        {
            duration = baseDuration;
        }


        /// <summary>
        /// Changes an effect' base variable based on the passed parameter.
        /// </summary>
        /// <param name="modifier"></param>
        public virtual void SetModifier(ModifierInfo modifier)
        {
            switch (modifier.name)
            {
                case "d": duration = baseDuration + modifier.value; break;
            }
        }
        public virtual float GetMaxDuration()
        {
            if (duration == 0)  //infinite
                return -1;

            return duration;
        }
        public abstract bool CanApplyEffectTo(CharacterData character, TAnimCore animator);
        public virtual bool CanApplyEffectTo(MyCharacterData character, RectTransform rect) { return true; }
        public abstract void ApplyEffectTo(ref CharacterData character, TAnimCore animator);
        public virtual void ApplyEffectTo(ref MyCharacterData character, RectTransform rect) { }
        public virtual void ApplyEffectToBehaviorDuration(ref MyCharacterData character, RectTransform rect) { }

        public virtual bool IsBehaviorDuration() { return false; }
    }

}