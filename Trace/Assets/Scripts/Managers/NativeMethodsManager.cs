using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NativeMethodsManager
{
    
    public static void OpenCamera(NativeCamera.CameraCallback callback )
    {

#if UNITY_IOS
        NativeCamera.Permission permission = NativeCamera.CheckPermission(true);

        if (permission== NativeCamera.Permission.Denied)
            NativeCamera.OpenSettings();
        if (permission == NativeCamera.Permission.ShouldAsk)
            NativeCamera.RequestPermission(true);
        
        if (permission == NativeCamera.Permission.Granted)
            NativeCamera.TakePicture(callback, 512, true);
#endif
    }



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
