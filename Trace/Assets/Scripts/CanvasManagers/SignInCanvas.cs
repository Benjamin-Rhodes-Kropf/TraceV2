using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SignInCanvas : MonoBehaviour
{
    [Header("Canvas Components")]
    [SerializeField] private TMP_InputField username;
    [SerializeField] private TMP_InputField password;
    
    private void OnEnable()
    {
        Debug.Log("Sign In Canvas Enabled");
    }

    public void LoginButtonHit()
    {
        StartCoroutine(FbManager.instance.Login(username.text, password.text, (myReturnValue) => {
            if (myReturnValue.IsSuccessful)
            {
                Debug.Log("FbManager: Logged in!");
                ScreenManager.instance.ChangeScreenDown("HomeScreen");
            }
            else
            {
                Debug.LogError("FbManager: failed to auto login");
            }
        }));
    }
}
