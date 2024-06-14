using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ThrowCommand : MonoBehaviour
{
    public UnityEvent eventAction;

    private void Awake()
    {
        eventAction?.Invoke();
    }
}
