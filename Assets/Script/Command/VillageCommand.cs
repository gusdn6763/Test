using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class VillageCommand : MultiTreeCommand
{
    protected AnimationHandler animationHandler;

    protected override void Awake()
    {
        animationHandler = GetComponent<AnimationHandler>();
        base.Awake();
    }
    public override void Interaction(MouseStatus mouseStatus)
    {
        base.Interaction(mouseStatus);
        StartCoroutine(animationHandler.AnimaionCoroutine(this, CurrentMouseStatus));
    }
}
