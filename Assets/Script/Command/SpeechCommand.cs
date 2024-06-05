using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class SpeechCommand : MultiTreeCommand
{
    public bool isFade = false;

    public void FadeText()
    {
        StopAllCoroutines();
        StartCoroutine(FadeTextCoroutine());
    }
    private IEnumerator FadeTextCoroutine()
    {
        IsAppearanceStart = false;
        IsDisAppearanceStart = true;
        isFade = true;

        currentDisAppearance = animationDataController.GetDisAppearanceTags();
        ConvertText(text.text);

        for (int i = 0; i < charactersCount; i++)
        {
            if (characters[i].isVisible)
            {
                characters[i].appearanceTime = characters[i].disappearancesMaxDuration;

                SetVisibilityChar(i, false);
                if (currentDisAppearance.waitForNormalChars != 0)
                    yield return new WaitForSeconds(currentDisAppearance.waitForNormalChars);
            }
        }

        for (int i = 0; i < charactersCount; i++)
        {
            yield return new WaitUntil(() => characters[i].appearanceTime <= 0);
        }

        yield return new WaitUntil(() => ChildCommands.All(cmd => cmd.IsDisAppearanceStart == false));

        IsDisAppearanceStart = false;
        isFade = false;
        gameObject.SetActive(false);
    }
}
