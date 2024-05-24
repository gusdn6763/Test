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
                command.CurrentArea.IsWait = true;
                yield return AnimationManager.instance.InitialAnimationCoroutine(command.ChildCommands);
                command.CurrentArea.IsWait = false;
                break;
            case MouseStatus.Excute:
                command.CurrentArea.IsWait = true;
                yield return AnimationManager.instance.CommandAllDisable(command);
                command.CurrentArea.IsWait = false;
                break;
            case MouseStatus.Exit:
                command.CurrentArea.IsWait = true;
                yield return AnimationManager.instance.AnimationCoroutine(command.ChildCommands, false);
                command.CurrentArea.IsWait = false;
                break;
        }
        AnimationEvent(command, mouseStatus);
    }

    public override void AnimationEvent(MultiTreeCommand command,MouseStatus mouseStatus)
    {
        AnimationManager.instance.DisableCommand();
        command.onAnimationEndEvent?.Invoke(mouseStatus);
    }
}