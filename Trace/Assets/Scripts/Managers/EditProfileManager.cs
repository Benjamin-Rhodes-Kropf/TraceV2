using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using SA.iOS.UIKit;
using System;

public class EditProfileManager : MonoBehaviour
{
    [SerializeField]private TMP_Text[] displayName, userName;
    [SerializeField]private TMP_Text emailId;

    public RawImage profileImage;

    Action<bool, string> callback;

    void Start()
    {
        profileImage.texture = FbManager.instance.userImageTexture;
        foreach (var item in displayName)
        {
            item.text = FbManager.instance.thisUserModel.DisplayName;
        }
        foreach (var item in userName)
        {
            item.text = FbManager.instance.thisUserModel.Username;
        }
        emailId.text = FbManager.instance.thisUserModel.Email;
        callback += CallBackFunction;

    }

    public void OpenGalleryForProfilePictureSelection()
    {
        //#if UNITY_EDITOR
        //        StartCoroutine(FbManager.instance.UploadProfilePhoto("Assets/Resources/profileimage.png", callback));
        //#elif UNITY_IOS
        ISN_UIImagePickerController picker = new ISN_UIImagePickerController();
        picker.SourceType = ISN_UIImagePickerControllerSourceType.Album;
        picker.MediaTypes = new List<string>() { ISN_UIMediaType.IMAGE };
        picker.MaxImageSize = 512;
        picker.ImageCompressionFormat = ISN_UIImageCompressionFormat.JPEG;
        picker.ImageCompressionRate = 0.8f;

        picker.Present((result) => {
            if (result.IsSucceeded)
            {
                Debug.Log("IMAGE local path: " + result.ImageURL);
                //m_image.sprite = result.Image.ToSprite();
                profileImage.texture = result.Image;
                StartCoroutine(FbManager.instance.UploadProfilePhoto(result.ImageURL, callback));

            }
            else
            {
                Debug.Log("Madia picker failed with reason: " + result.Error.Message);
            }
        });
        //Debug.Log("Assets/Resources/profileimage.png");
        //#endif
    }
    void CallBackFunction(bool flag, string path)
    {

    }
}
