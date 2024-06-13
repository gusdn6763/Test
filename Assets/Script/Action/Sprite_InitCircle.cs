using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Sprite_InitCircle : Action_Command
{
    private GameObject initCircle;

    public bool isFirst = true;

    protected override void Awake()
    {
        base.Awake();
        initCircle = Instantiate(PrefabManager.instance.initCircle, transform);
    }

    public GameObject CreateInitCircle()
    {
        return Instantiate(PrefabManager.instance.initCircle, transform);
    }

    public override void MouseEvent(MouseStatus mouseStatus)
    {
        switch(mouseStatus)
        {
            case MouseStatus.Enter:
                initCircle.SetActive(false);
                break;
        }
    }



    public override void ConditionEvent(bool isOn)
    {
        if (isFirst && isOn)
        {
            isFirst = false;
            initCircle.SetActive(true);

            if (command.ParentCommand)
            {
                Sprite_InitCircle parentInitCircle = command.ParentCommand.interaction.GetComponent<Sprite_InitCircle>();

                if (parentInitCircle)
                    parentInitCircle.initCircle.SetActive(true);   
            }
        }
    }
}
