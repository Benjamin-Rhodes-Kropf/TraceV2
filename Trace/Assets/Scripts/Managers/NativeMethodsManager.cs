using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NativeMethodsManager
{

    public static void OpenGalleryToPickMedia(NativeGallery.MediaPickCallback callback)
    {
#if UNITY_IOS
        NativeGallery.Permission permission = NativeGallery.CheckPermission(NativeGallery.PermissionType.Read,NativeGallery.MediaType.Image);

        if (permission== NativeGallery.Permission.Denied)
            NativeGallery.OpenSettings();
        if (permission == NativeGallery.Permission.ShouldAsk)
            NativeGallery.RequestPermission(NativeGallery.PermissionType.Read,NativeGallery.MediaType.Image);

        if (permission == NativeGallery.Permission.Granted)
            NativeGallery.GetImageFromGallery(callback);
#endif
    }
  

}
