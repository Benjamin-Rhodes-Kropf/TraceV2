using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingUpAccount : MonoBehaviour
{
    private void OnEnable()
    {
        UploadProfilePicture();
    }


    private void UploadProfilePicture()
    {
        if (TookPhotoCanvasController._profilePicture == null)
            return;
        
        MyDebug.Instance.LogError("Upload Image Called");
        var bytes = TookPhotoCanvasController._profilePicture.texture.EncodeToPNG();
        StartCoroutine(FbManager.instance.UploadProfilePhoto(bytes, (isUploaded, url) =>
        {
            MyDebug.Instance.LogError("Upload Profile Photo Bytes URL :: "+ url);
            if (isUploaded)
            {
                StartCoroutine(FbManager.instance.SetUserProfilePhotoUrl(url,
                    (isUrlSet) =>
                    {
                        if (isUrlSet)
                        {
                            ScreenManager.instance.ChangeScreenFade("HomeScreen");
                            print("Called");
                        }
                        else
                        {
                            MyDebug.Instance.LogError("URL Off Image is :: "+ url);
                        }
                    }));
            }
        }));
    }
    
}
