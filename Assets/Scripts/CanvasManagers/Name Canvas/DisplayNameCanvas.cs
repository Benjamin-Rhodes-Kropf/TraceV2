using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DisplayNameCanvas : MonoBehaviour
{
    public TMP_InputField _displayNameInputField;
    public TMP_InputField _username;
    public Button _submitButton;
    public TMP_Text errorText;

    
    private DisplayNameCanvasController _controller;

    private void OnEnable()
    {

        if (_controller == null)
            _controller = new DisplayNameCanvasController(this);
            
        _controller.Init();
    }
    
    
     
    public void ShowMessage(string message)
    {
        errorText.text = message;
        errorText.gameObject.SetActive(true);
            
        StartCoroutine(HelperMethods.TimedActionFunction(3f, ()=>
        {
            errorText.text = "";
            errorText.gameObject.SetActive(false);
        }));
    }


    private void OnDisable()
    {
        _controller.Uninitialise();
    }
}
