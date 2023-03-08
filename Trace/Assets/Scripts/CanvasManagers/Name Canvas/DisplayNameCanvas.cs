using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DisplayNameCanvas : MonoBehaviour
{
    public TMP_InputField _displayNameInputField;
    public Button _submitButton;

    
    private DisplayNameCanvasController _controller;

    private void OnEnable()
    {

        if (_controller == null)
            _controller = new DisplayNameCanvasController(this);
            
        _controller.Init();
    }

    private void OnDisable()
    {
        _controller.Uninitialise();
    }
}
