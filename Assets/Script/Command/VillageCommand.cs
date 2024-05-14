using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class VillageCommand : MultiTreeCommand
{
    public SpriteRenderer initCircle { get; set; }
    public bool IsFirstShow { get; set; } = true;
    public override bool IsInitCircleEnabled
    {
        set
        {
            initCircle.enabled = value;
            if (ParentCommand != null)
                ParentCommand.IsInitCircleEnabled = value;
        }
    }

    protected override void Awake()
    {
        initCircle = GetComponentInChildren<SpriteRenderer>(true);
        base.Awake();
    }

    #region 상호작용

    protected void OnMouseEnter()
    {
        initCircle.enabled = false;
    }
    public override void Interaction(MouseStatus mouseStatus)
    {
        StartCoroutine(AnimaionCoroutine(mouseStatus));
    }

    IEnumerator AnimaionCoroutine(MouseStatus mouseStatus)
    {
        switch (mouseStatus)
        {
            case MouseStatus.Enter:
                rigi.velocity = Vector3.zero;
                yield return AnimationManager.instance.InitialAnimationCoroutine(ChildCommands);
                break;
            case MouseStatus.Exit:
                yield return AnimationManager.instance.AnimationCoroutine(ChildCommands, false);
                break;
        }
        onMouseEvent?.Invoke(mouseStatus);
    }

    #endregion
}