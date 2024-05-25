using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteList : MonoBehaviour
{
    private RectTransform rectTrans;

    public void TryInitializing()
    {
        rectTrans = GetComponent<RectTransform>();
    }

    public void SetSize(Vector3 size)
    {
        rectTrans.sizeDelta = size;
    }
}
