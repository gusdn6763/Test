using System;
using System.Collections;
using UnityEngine;

public abstract class AnimationHandler : MonoBehaviour
{
    public Action<MouseStatus> onAnimationEvent;
    public abstract IEnumerator AnimaionCoroutine(MultiTreeCommand command, MouseStatus mouseStatus);

    public abstract void AnimationEvent(MouseStatus mouseStatus);
}