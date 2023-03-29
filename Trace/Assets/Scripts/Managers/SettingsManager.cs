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
    //Textfields references
    [SerializeField]
    private TMP_Text profileName;
    [SerializeField]
    private TMP_Text userName;

    //profile pic reference
    public RawImage profileImage;

    //toggle ui elements for notification
    [SerializeField]private Sprite toggleOn, toggleOff;
    [SerializeField] private Button notificationButton;

    //action used for callback on firebase data handling
    Action<bool, string> callbackForProfilePicUpdate;
    Action<Texture> OnGetProfilePhotoFromFirebaseStorage;

    //logout popup temperory
    public GameObject logOutPopUp;

    void OnEnable()
    {
        //assiging the latest profile picture when this screen is enabled
        if(FbManager.instance.userImageTexture!=null)
            profileImage.texture = FbManager.instance.userImageTexture;
        //check expection if userdata is not null
        if (FbManager.instance.thisUserModel != null)
        {
            //asigning the values f displayname and username from the firebase db
            profileName.text = FbManager.instance.thisUserModel.DisplayName;
            userName.text = FbManager.instance.thisUserModel.Username;
        }
    }
    void Start()
    {
        //setting toggle preprefs
        if (!PlayerPrefs.HasKey("notificationStatus"))
            PlayerPrefs.SetInt("notificationStatus", 1);

        //setting up the toggle sprite values accoring to the playerprefs
        if (PlayerPrefs.GetInt("notificationStatus") == 1)
            notificationButton.image.sprite = toggleOn;
        else
            notificationButton.image.sprite = toggleOff;
        //assigning callbacks/actions required by firebase to handle data on success
        callbackForProfilePicUpdate += CallBackFunction;
        OnGetProfilePhotoFromFirebaseStorage += CallBackFunctionOnImageRetriveFromDatabase;

        //getting the profile image from the firebase db for user
        FbManager.instance.GetProfilePhotoFromFirebaseStorage(FbManager.instance.thisUserModel.userId, OnGetProfilePhotoFromFirebaseStorage);
    }
    //updating the user profile image
    public void OpenGalleryForProfilePictureSelection() {
#if UNITY_EDITOR //for testing on editor
        StartCoroutine(FbManager.instance.UploadProfilePhoto("Assets/Resources/profileimage1.png", callbackForProfilePicUpdate));
#elif UNITY_IOS //for iOS device, opens up the gallary and image is picked and updated to user profile on firebase
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
                StartCoroutine(FbManager.instance.UploadProfilePhoto(result.ImageURL, callbackForProfilePicUpdate));

            }
            else
            {
                Debug.Log("Madia picker failed with reason: " + result.Error.Message);
            }
        });
#endif
    }

    void CallBackFunction(bool flag, string path) {
        FbManager.instance.GetProfilePhotoFromFirebaseStorage(FbManager.instance.thisUserModel.userId, OnGetProfilePhotoFromFirebaseStorage);

        //assigning the userImageTexture value to the user profile pic texture, so it can be used and assigned every where
        //FbManager.instance.userImageTexture = _profileImage;
    }
    void  CallBackFunctionOnImageRetriveFromDatabase(Texture _profileImage) {
        if (_profileImage != null)
        {
            //assigning the userImageTexture value to the user profile pic texture, so it can be used and assigned every where
            FbManager.instance.userImageTexture = _profileImage;
            //assiging this screen profile pic
            profileImage.texture = FbManager.instance.userImageTexture;
        }
    }
    //opens up the url for about button
    public void About() {
        Application.OpenURL("https://www.leaveatrace.app/");
    }
    //opens up the email for contactus button
    public void ContactUs()
    {
        string email = "trace.contact@gmail.com";
        string subject = MyEscapeURL("Contact Us");
        string body = MyEscapeURL("Hi\r\n My name is "+FbManager.instance.thisUserModel.DisplayName);
        Application.OpenURL("mailto:" + email + "?subject=" + subject + "&body=" + body);
    }
    //opens up the GetHelp for contactus button
    public void GetHelp()
    {
        string email = "trace.help@gmail.com";
        string subject = MyEscapeURL("Get Help");
        string body = MyEscapeURL("Hi\r\n My name is " + FbManager.instance.thisUserModel.DisplayName);
        Application.OpenURL("mailto:" + email + "?subject=" + subject + "&body=" + body);
    }
    string MyEscapeURL(string url)
    {
        return WWW.EscapeURL(url).Replace("+", "%20");
    }
    //change the toggle image,
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
    //it will show the popup for logout
    public void LogOut() {
        logOutPopUp.SetActive(true);
    }
    //yes and no pop up for logout purpose
    public void ConfirmationLogOut(bool consent) {
        if (consent)
        {
            //logout user, clear all the playerprefs and open welcome screen
            FbManager.instance.LogOutOfAccount();
            logOutPopUp.SetActive(false);
        }
        else
            logOutPopUp.SetActive(false);//when clicked no, close the popup
    }
    void ShowCorfirmation(string dateUpdateType)
    {
        ISN_UIAlertController alert = new ISN_UIAlertController("Data Update", dateUpdateType + " updated successfully!", ISN_UIAlertControllerStyle.Alert);
        ISN_UIAlertAction defaultAction = new ISN_UIAlertAction("Ok", ISN_UIAlertActionStyle.Default, () => {
            //Do something
        });

        alert.AddAction(defaultAction);
        alert.Present();
    }
    /*
    void UpdateUserDetailsFromFirebase() {
        FbManager.instance.GetProfilePhotoFromFirebaseStorage(FbManager.instance.thisUserModel.userId, OnGetProfilePhotoFromFirebaseStorage);
        print(FbManager.instance.thisUserModel.DisplayName);
    }*/

}
