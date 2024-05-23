using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(AnimationHandler))]
public class VillageCommand : MultiTreeCommand
{
    protected AnimationHandler animationHandler;
    public override void Interaction(MouseStatus mouseStatus)
    {
        base.Interaction(mouseStatus);
        StartCoroutine(animationHandler.AnimaionCoroutine(this, currentMouseStatus));
    }
}
