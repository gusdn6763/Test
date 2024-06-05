using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuList : MonoBehaviour
{
    private RectTransform rectTrans;
    public void TryInitializing(Vector2 pos)
    {
        rectTrans = GetComponent<RectTransform>();

        Vector2 vector = new Vector2(0, -(pos.y * 0.5f));

        rectTrans.anchoredPosition = vector;
    }
}
