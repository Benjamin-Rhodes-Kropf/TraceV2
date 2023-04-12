using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class EditProfileManager : MonoBehaviour
{
    [SerializeField]private TMP_Text[] displayName, userName;
    [SerializeField]private TMP_Text emailId;

    public RawImage profileImage;
    void Start()
    {
        profileImage.texture = FbManager.instance.userImageTexture;
        foreach (var item in displayName)
        {
            item.text = FbManager.instance._currentUser.DisplayName;
        }
        foreach (var item in userName)
        {
            item.text = FbManager.instance._currentUser.Username;
        }
        emailId.text = FbManager.instance._currentUser.Email;

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
