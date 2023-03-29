using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using SA.iOS.UIKit;
using SA.iOS.AVFoundation;

public class SettingsManager : MonoBehaviour
{
    [SerializeField]
    private TMP_Text profileName;
    [SerializeField]
    private TMP_Text userName;

    public RawImage profileImage;
    [SerializeField]private Sprite toggleOn, toggleOff;
    [SerializeField] private Button notificationButton;
    Action<bool, string> callback;
    Action<Texture> onSuccess;
    private Texture myPicture;
    public GameObject logOutPopUp;

    void Start()
    {
        if (!PlayerPrefs.HasKey("notificationStatus"))
            PlayerPrefs.SetInt("notificationStatus", 1);
        if (PlayerPrefs.GetInt("notificationStatus") == 1)
            notificationButton.image.sprite = toggleOn;
        else
            notificationButton.image.sprite = toggleOff;

        callback += CallBackFunction;
        onSuccess += CallBackFunctionOnImageRetriveFromDatabase;
        profileName.text = FbManager.instance.thisUserModel.DisplayName;
        userName.text = FbManager.instance.thisUserModel.Username;
        FbManager.instance.GetProfilePhotoFromFirebaseStorage(FbManager.instance.thisUserModel.userId, onSuccess);
    }

    public void OpenGalleryForProfilePictureSelection() {
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
    void CallBackFunction(bool flag, string path) {

    }
    void  CallBackFunctionOnImageRetriveFromDatabase(Texture _profileImage) {
        FbManager.instance.userImageTexture = _profileImage;
        profileImage.texture = FbManager.instance.userImageTexture;

        //return _profileImage;
    }
    public void About() {
        Application.OpenURL("https://www.leaveatrace.app/");
    }
    public void ContactUs()
    {
        string email = "me@example.com";
        string subject = MyEscapeURL("Contact Us");
        string body = MyEscapeURL("My Body\r\nFull of non-escaped chars");
        Application.OpenURL("mailto:" + email + "?subject=" + subject + "&body=" + body);
    }
    public void GetHelp()
    {
        string email = "me@example.com";
        string subject = MyEscapeURL("Get Help");
        string body = MyEscapeURL("My Body\r\nFull of non-escaped chars");
        Application.OpenURL("mailto:" + email + "?subject=" + subject + "&body=" + body);
    }
    string MyEscapeURL(string url)
    {
        return WWW.EscapeURL(url).Replace("+", "%20");
    }
    public void ToggleNotificationSetting() {
        if (PlayerPrefs.GetInt("notificationStatus") == 1)
        {
            PlayerPrefs.SetInt("notificationStatus", 0);
            notificationButton.image.sprite = toggleOff;
        }
        else
        {
            PlayerPrefs.SetInt("notificationStatus", 1);
            notificationButton.image.sprite = toggleOn;
        }
    }
    public void LogOut() {
        logOutPopUp.SetActive(true);
    }
    public void ConfirmationLogOut(bool consent) {
        if (consent)
        {
            FbManager.instance.LogOutOfAccount();
            logOutPopUp.SetActive(false);
        }
        else
            logOutPopUp.SetActive(false);
    }
    
}
