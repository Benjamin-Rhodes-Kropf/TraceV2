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
    [SerializeField] private TMP_Text errorText;
    
    private void OnEnable()
    {
        Debug.Log("Sign In Canvas Enabled");
    }

    public void LoginButtonHit()
    {
        StartCoroutine(FbManager.instance.Login(username.text, password.text, (myReturnValue) => {
            if (myReturnValue.IsSuccessful)
                ScreenManager.instance.ChangeScreenNoAnim("HomeScreen");
            else
                ShowMessage("Your Email or Password is incorrect !");
        }));
    }


    private void ShowMessage(string message)
    {
        errorText.text = message;
        errorText.gameObject.SetActive(true);
            
        StartCoroutine(HelperMethods.TimedActionFunction(3f, ()=>
        {
            errorText.text = "";
            errorText.gameObject.SetActive(false);
        }));
    }
}
