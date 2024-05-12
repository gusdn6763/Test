using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class IntroduceText : MonoBehaviour
{
    public TextMeshProUGUI text;
   
    [SerializeField] private Color defaultColor;
    [SerializeField] private Color previewColor;

    private void Start()
    {
        text.color = defaultColor;
    }

    public void ColorChange()
    {
        text.color = previewColor;
    }
}
