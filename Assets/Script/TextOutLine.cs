using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextOutLine : MonoBehaviour
{
    private MultiTreeCommand multiTreeCommand;
    private RectTransform rectTransform;

    private void Awake()
    {
        multiTreeCommand = GetComponentInParent<MultiTreeCommand>();
        rectTransform = GetComponent<RectTransform>();
    }

    private void OnEnable()
    {
        rectTransform.localScale = multiTreeCommand.GetSize();
    }
}
