using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public class VillageAnimationHandler : AnimationHandler
{
    public override IEnumerator AnimaionCoroutine(MultiTreeCommand command, MouseStatus mouseStatus)
    {
        switch (mouseStatus)
        {
            case MouseStatus.Enter:
                yield return AnimationManager.instance.InitialAnimationCoroutine(command.ChildCommands);
                break;
            case MouseStatus.Excute:
                yield return AnimationManager.instance.CommandAllDisable(command);
                break;
            case MouseStatus.Exit:
                yield return AnimationManager.instance.AnimationCoroutine(command.ChildCommands, false);
                break;
        }
        AnimationEvent(mouseStatus);
    }

    public override void AnimationEvent(MouseStatus mouseStatus)
    {
        AnimationManager.instance.DisableCommand();
        onAnimationEvent?.Invoke(mouseStatus);
    }
}