using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    [SerializeField] private GameObject blurObj;

    public bool BlurOn { get { return blurObj.activeSelf; } }
    
    protected void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != null)
            Destroy(gameObject);
    }

    public void Blur(bool isOn)
    {
        blurObj.SetActive(isOn);
    }
}
