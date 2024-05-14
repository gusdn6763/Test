using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public class RootVillageAnimationHandler : AnimationHandler
{
    public override IEnumerator AnimaionCoroutine(MultiTreeCommand command, MouseStatus mouseStatus)
    {
        switch (mouseStatus)
        {
            case MouseStatus.Enter:
                yield return StartCoroutine(AnimationManager.instance.IsBehaviorAnimationCoroutine(command));
                yield return StartCoroutine(AnimationManager.instance.InitialAnimationCoroutine(command.ChildCommands));
                break;
            case MouseStatus.Down:
                yield return StartCoroutine(AnimationManager.instance.AnimationCoroutine(command.ChildCommands, false));
                yield return StartCoroutine(AnimationManager.instance.SeparatorCoroutine(command, true));
                break;
            case MouseStatus.Up:
                yield return StartCoroutine(AnimationManager.instance.SeparatorCoroutine(command, false));
                break;
            case MouseStatus.Drag:
                transform.position = MoveCommand();
                break;
            case MouseStatus.Excute:
                yield return AnimationManager.instance.CommandAllDisable(command);
                break;
            case MouseStatus.Exit:
                yield return StartCoroutine(AnimationManager.instance.AnimationCoroutine(command.ChildCommands, false));
                yield return StartCoroutine(AnimationManager.instance.IsBehaviorAnimationCoroutine(command));
                break;
        }
        AnimationEvent(mouseStatus);
    }

    public override void AnimationEvent(MouseStatus mouseStatus)
    {
        AnimationManager.instance.DisableCommand();
        onAnimationEvent?.Invoke(mouseStatus);
    }

    public Vector3 MoveCommand()
    {
        Vector3 mousePoint = Input.mousePosition;
        mousePoint.z = Camera.main.WorldToScreenPoint(transform.position).z;

        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(mousePoint);

        // 카메라 화면 범위 내에서 마우스 위치 제한
        Vector3 viewportPosition = Camera.main.WorldToViewportPoint(worldPosition);
        viewportPosition.x = Mathf.Clamp01(viewportPosition.x);
        viewportPosition.y = Mathf.Clamp01(viewportPosition.y);

        // 제한된 마우스 위치를 다시 월드 좌표로 변환
        Vector3 clampedWorldPosition = Camera.main.ViewportToWorldPoint(viewportPosition);
        clampedWorldPosition.z = transform.position.z;

        return clampedWorldPosition;
    }
}