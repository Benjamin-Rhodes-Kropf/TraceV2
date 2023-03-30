using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EditProfileCanvasView : MonoBehaviour
{
    public Image _profilePicture;
    public TMP_Text _profileNameLarge;
    public TMP_Text _usernameLarge;

    public TMP_Text _profileName;
    public TMP_Text _username;
    public TMP_Text _email;
    public TMP_Text _password;
    
    
    private EditProfileCanvasViewController _controller;


    #region UnityEvents

    private void OnEnable()
    {
        if (_controller == null)
            _controller = new EditProfileCanvasViewController();
            
        _controller.Init(this);
    }

    private void OnDisable()
    {
        _controller.UnInitialize();
    }

    #endregion

}
