using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sprite_InitCircle : Action_Command
{
    private GameObject initCircle;
    private bool isFirst = true;
    public bool IsOn { get => initCircle.activeSelf; set => initCircle.SetActive(value); }

    private void Start()
    {
        initCircle = Instantiate(PrefabManager.instance.initCircle, transform);
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

            Sprite_InitCircle[] parentInitCircles = GetComponentsInParent<Sprite_InitCircle>();
            foreach (Sprite_InitCircle parentInitCircle in parentInitCircles)
            {
                if (parentInitCircle != this)
                    parentInitCircle.IsOn = true;
            }
        }
    }
}
