using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemRootCommand : MultiTreeCommand
{
    protected override void ScritibleInit()
    {
        base.ScritibleInit();

        if (multiTreeData is ItemData)
        {
            ItemData itemData = multiTreeData as ItemData;
        }
    }

    public override void Interaction(MouseStatus mouseStatus)
    {
        //StartCoroutine(AnimaionCoroutine(mouseStatus));
        onMouseEvent?.Invoke(mouseStatus);
    }
}
