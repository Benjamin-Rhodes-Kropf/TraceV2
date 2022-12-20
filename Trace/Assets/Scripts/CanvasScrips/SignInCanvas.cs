using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SignInCanvas : MonoBehaviour
{
    [Header("Screen Manager")]
    [SerializeField] private ScreenManager _screenManager;
    
    [Header("Canvas Components")]
    [SerializeField] private TMP_InputField username;
    [SerializeField] private TMP_InputField password;
    
    private void OnEnable()
    {
        Debug.Log("Sign In Canvas Enabled");
    }

    public void LoginButtonHit()
    {
        StartCoroutine(fakeLoginTime());
    }
    
    
    IEnumerator fakeLoginTime()
    {
        yield return new WaitForSeconds(2f);
        _screenManager.ChangeScreenDown("HomeScreen");
    }
}
