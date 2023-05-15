using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyDebug : MonoBehaviour
{
    [SerializeField] private bool isTesting = true;
    private static MyDebug instance = null;
    public static MyDebug Instance
    {
        get
        {
            if (instance == null)
                instance = new MyDebug();

            return instance;
        }
    }
    private MyDebug()
    {
        // Private Constructor For Singleton
    }

    public void LogError(string message, GameObject obj = null)
    {
        if (!isTesting)
            return;
        
        if (obj)
            Debug.LogError(message, obj);
        else
            Debug.LogError(message);
    }
    
    public void Log(string message, GameObject obj = null)
    {
        if (!isTesting)
            return;
        
        if (obj)
            Debug.Log(message, obj);
        else
            Debug.Log(message);
    }
    
    public void LogWarning(string message, GameObject obj = null)
    {
        if (!isTesting)
            return;
        
        if (obj)
            Debug.LogWarning(message, obj);
        else
            Debug.LogWarning(message);
    }
}
