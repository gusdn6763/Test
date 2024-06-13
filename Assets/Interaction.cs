using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interaction : MonoBehaviour
{
    public MultiTreeCommand command;

    private RectTransform rectTrans;
    private BoxCollider box;


    private void Awake()
    {
        rectTrans = GetComponent<RectTransform>();
        box = GetComponent<BoxCollider>();
    }

    public void SetSize(Vector3 size)
    {
        rectTrans.sizeDelta = size;
        box.size = size;
    }
}
