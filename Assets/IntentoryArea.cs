using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntentoryArea : Area
{
    public static IntentoryArea instance;

    protected void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != null)
            Destroy(gameObject);
    }
}
