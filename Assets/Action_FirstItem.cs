using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Action_FirstItem : Action_Command
{
    [SerializeField] private ItemCommand item;
    [SerializeField] private MoveCommand location;

    public override void AnimationEvent(MouseStatus mouseStatus) 
    {
        switch(mouseStatus)
        {
            case MouseStatus.Excute:
                command.onAnimationEndEvent.RemoveListener(AnimationEvent);
                GameManager.instance.currentArea.CreateItem(location, item);
                break;
        }
    }
}
