using Febucci.UI.Core;
using UnityEngine;

[RequireComponent(typeof(MultiTreeCommand))]
public abstract class Action_Command : MonoBehaviour
{
    protected MultiTreeCommand command;

    protected virtual void Awake()
    {
        command = GetComponent<MultiTreeCommand>();
        command.onMouseEvent += MouseEvent;
    }

    public abstract void MouseEvent(MouseStatus mouseStatus);
}