using UnityEngine;
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
        box.isTrigger = true;
    }

    #region 상호작용

    protected void OnMouseEnter()
    {
        initCircle.enabled = false;
    }

    public override void Interaction(MouseStatus mouseStatus)
    {
        switch (mouseStatus)
        {
            case MouseStatus.Enter:
                rigi.velocity = Vector3.zero;
                //동기식?
                AnimationManager.instance.IsBehaviorAnimation(this);
                AnimationManager.instance.InitialAnimation(ChildCommands, true);
                break;
            case MouseStatus.Excute:
                Excute();
                break;
            case MouseStatus.Exit:
                AnimationManager.instance.IsBehaviorAnimation(this);
                AnimationManager.instance.Animation(ChildCommands, false);
                break;
        }
        onMouseEvent?.Invoke(mouseStatus);
    }

    public override void Excute()
    {
        
    }

    #endregion
}