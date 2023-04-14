using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingUpAccount : MonoBehaviour
{
    [SerializeField] private ScreenManager _screenManager;
    private void OnEnable()
    {
        UploadProfilePicture();
    }


    private void UploadProfilePicture()
    {
        var bytes = TookPhotoCanvasController._profilePicture.texture.EncodeToPNG();
        StartCoroutine(FbManager.instance.UploadProfilePhoto(bytes, (isUploaded, url) =>
        {
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
                    }));
            }
        }));
    }
    
}
