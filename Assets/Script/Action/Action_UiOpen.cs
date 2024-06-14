using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

[System.Serializable]
public struct UiStatus
{
    public MouseStatus mouseStatus;
    public UIScript uIScript;
}

public class Action_UiOpen : Action_Command
{
    [SerializeField] GameObject test;
    public GameObject camera2;
    public Area activeArea;
    public Area disActiveArea;

    public override void AnimationEvent(MouseStatus mouseStatus)
    {
        if (mouseStatus == MouseStatus.Excute)
        {
            camera2.transform.position = new Vector3(50, 0, 0);
            test.gameObject.SetActive(true);
            disActiveArea.start = false;
            activeArea.start = true;
        }
    }
}
