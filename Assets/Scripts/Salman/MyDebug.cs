using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyDebug : MonoBehaviour
{

    public static MyDebug Instance;

    private void Awake()
    {
        Instance = this;
    }

    //public LayerMask layerMaskForMapDetection;
}
