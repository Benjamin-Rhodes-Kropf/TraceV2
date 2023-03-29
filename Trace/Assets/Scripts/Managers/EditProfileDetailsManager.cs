using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EditProfileDetailsManager : MonoBehaviour
{
    [SerializeField]
    private TMP_Text diplayName;
    [SerializeField]
    private TMP_Text userName;
    
    public RawImage profilePicture;

    [SerializeField]
    private TMP_Text editableField;
    [SerializeField] private bool isDisplayNameScreen, isUserNameScreen, isEmailScreen, isPasswordScreen;



    
    void Start()
    {
        profilePicture.texture = FbManager.instance.userImageTexture;
        diplayName.text = FbManager.instance.thisUserModel.DisplayName;
        userName.text = FbManager.instance.thisUserModel.Username;

    }
    
    public void ApplyAndSave()
    {

    }
    
}
