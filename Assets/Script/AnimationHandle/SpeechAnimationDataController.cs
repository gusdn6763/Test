using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SpeechAnimationDataController : AnimationDataController
{
    public override DisAppearanceAnimationScriptible GetDisAppearanceTags()
    {
        if (command is SpeechCommand)
        {
            SpeechCommand speechCommand = command as SpeechCommand;

            if (speechCommand.isFade)
                return animationData.FindDisAppearanceType(AnimationConditionType.Speech_FadeIn);
        }

        return animationData.FindDisAppearanceType(AnimationConditionType.Default);
    }
}