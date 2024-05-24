using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class RootVillageAnimationHandler : AnimationHandler
{
    private Rigidbody rigi;
    private float defaultMass;
    private float drfaultDrag;

    private void Awake()
    {
        rigi = GetComponent<Rigidbody>();
        defaultMass = rigi.mass;
        drfaultDrag = rigi.drag;
    }

    public override IEnumerator AnimaionCoroutine(MultiTreeCommand command, MouseStatus mouseStatus)
    {
        switch (mouseStatus)
        {
            case MouseStatus.Enter:
                command.CurrentArea.IsWait = true;
                yield return StartCoroutine(AnimationManager.instance.IsBehaviorAnimationCoroutine(command));
                yield return StartCoroutine(AnimationManager.instance.InitialAnimationCoroutine(command.ChildCommands));
                command.CurrentArea.IsWait = false;
                break;
            case MouseStatus.Down:
                rigi.mass = 0;
                rigi.drag = 0;
                command.CurrentArea.IsWait = true;
                yield return StartCoroutine(AnimationManager.instance.AnimationCoroutine(command.ChildCommands, false));
                yield return StartCoroutine(AnimationManager.instance.SeparatorCoroutine(command, true));
                command.CurrentArea.IsWait = false;
                break;
            case MouseStatus.Up:
                rigi.velocity = Vector3.zero;
                command.CurrentArea.IsWait = true;
                yield return StartCoroutine(AnimationManager.instance.SeparatorCoroutine(command, false));
                command.CurrentArea.IsWait = false;
                break;
            case MouseStatus.Drag:
                rigi.mass = defaultMass;
                rigi.velocity = (MoveCommand() - transform.position).normalized * 10f;
                break;
            case MouseStatus.Excute:
                command.CurrentArea.IsWait = true;
                yield return AnimationManager.instance.CommandAllDisable(command);
                command.CurrentArea.IsWait = false;
                break;
            case MouseStatus.Exit:
                rigi.mass = defaultMass;
                command.CurrentArea.IsWait = true;
                yield return StartCoroutine(AnimationManager.instance.AnimationCoroutine(command.ChildCommands, false));
                yield return StartCoroutine(AnimationManager.instance.IsBehaviorAnimationCoroutine(command));
                command.CurrentArea.IsWait = false;
                break;
        }
        AnimationEvent(command, mouseStatus);
    }

    public override void AnimationEvent(MultiTreeCommand command, MouseStatus mouseStatus)
    {
        AnimationManager.instance.DisableCommand();
        command.onAnimationEndEvent?.Invoke(mouseStatus);
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

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("충돌");
        rigi.AddForce((transform.position - collision.transform.position).normalized);
    }
}