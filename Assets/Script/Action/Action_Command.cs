using UnityEngine;

public abstract class Action_Command : MonoBehaviour
{
    protected MultiTreeCommand command;

    protected virtual void Awake()
    {
        command = GetComponentInParent<MultiTreeCommand>();
        command.isConditionEvent += ConditionEvent;
        command.onMouseEvent += MouseEvent;
        command.onAnimationEndEvent += AnimationEvent;
    }

    public virtual void ConditionEvent(bool isOn) { }
    public virtual void MouseEvent(MouseStatus mouseStatus) { }
    public virtual void AnimationEvent(MouseStatus mouseStatus) { }
}