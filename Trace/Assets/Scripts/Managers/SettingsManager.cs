using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
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
#if UNITY_EDITOR
        StartCoroutine(FbManager.instance.UploadProfilePhoto("Assets/Resources/profileimage.png", callback));
#elif UNITY_IOS
        Debug.Log("Assets/Resources/profileimage.png");
#endif
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
}
