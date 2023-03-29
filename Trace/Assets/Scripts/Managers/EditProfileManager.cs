using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using SA.iOS.UIKit;
using System;

public class EditProfileManager : MonoBehaviour
{
    //Textfields references
    [SerializeField]private TMP_Text[] displayName, userName;
    [SerializeField]private TMP_Text emailId;

    //profile pic reference
    public RawImage profileImage;

    //action used for callback on firebase data handling
    Action<bool, string> callbackForProfilePicUpdate;
    Action<Texture> OnGetProfilePhotoFromFirebaseStorage;

    void OnEnable()
    {
        //assiging the latest profile picture when this screen is enabled
        if (FbManager.instance.userImageTexture != null)
            profileImage.texture = FbManager.instance.userImageTexture;

        //check expection if userdata is not null
        if (FbManager.instance.thisUserModel != null)
        {
            //assigning all the text field values
            foreach (var item in displayName)
            {
                item.text = FbManager.instance.thisUserModel.DisplayName;
            }

            //assigning all the text field values
            foreach (var item in userName)
            {
                item.text = FbManager.instance.thisUserModel.Username;
            }

            //assigning all the text field values
            emailId.text = FbManager.instance.thisUserModel.Email;
        }
    }
    void Start()
    {
        //assigning callbacks/actions required by firebase to handle data on success
        callbackForProfilePicUpdate += CallBackFunction;
        OnGetProfilePhotoFromFirebaseStorage += CallBackFunctionOnImageRetriveFromDatabase;

    }
    //updating the user profile image
    public void OpenGalleryForProfilePictureSelection()
    {
#if UNITY_EDITOR //for testing on editor
        StartCoroutine(FbManager.instance.UploadProfilePhoto("Assets/Resources/profileimage.png", callbackForProfilePicUpdate));
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
    void CallBackFunction(bool flag, string path)
    {
        FbManager.instance.GetProfilePhotoFromFirebaseStorage(FbManager.instance.thisUserModel.userId, OnGetProfilePhotoFromFirebaseStorage);

    }
    void CallBackFunctionOnImageRetriveFromDatabase(Texture _profileImage)
    {
        if (_profileImage != null)
        {
            //assigning the userImageTexture value to the user profile pic texture, so it can be used and assigned every where
            FbManager.instance.userImageTexture = _profileImage;
            //assiging this screen profile pic
            profileImage.texture = FbManager.instance.userImageTexture;
        }
    }
    //show native alert when clicked on username edit
    public void OpenPopOnUserNameEdit() {
        ISN_UIAlertController alert = new ISN_UIAlertController("Oops!", "we are sorry, but you can't change your username for now", ISN_UIAlertControllerStyle.Alert);
        ISN_UIAlertAction defaultAction = new ISN_UIAlertAction("Ok", ISN_UIAlertActionStyle.Default, () => {
            //Do something
        });

        alert.AddAction(defaultAction);
        alert.Present();
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
}
//com.traceco.tracesamplelocation