using Febucci.UI.Core;
using UnityEngine;

[RequireComponent(typeof(MultiTreeCommand), typeof(AnimationHandler))]
public abstract class Action_Command : MonoBehaviour
{
    protected MultiTreeCommand command;

    protected virtual void Awake()
    {
        command = GetComponent<MultiTreeCommand>();
        command.onMouseEvent.AddListener(MouseEvent);
        command.onAnimationEndEvent.AddListener(AnimationEvent);
    }

    public virtual void MouseEvent(MouseStatus mouseStatus) { }

    public virtual void AnimationEvent(MouseStatus mouseStatus) { }
}