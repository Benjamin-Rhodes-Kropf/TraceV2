using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TakePhotoCanvasController
{
    private TakePhotoCanvas _view;




    public TakePhotoCanvasController(TakePhotoCanvas view)
    {
        _view = view;
    }



    public void Init()
    {
        _view._cameraButton.onClick.AddListener(OnCameraButtonClick);
        _view._galleryButton.onClick.AddListener(OnGalleryClick);
    }



    public void Uninit()
    {
        _view._cameraButton.onClick.RemoveAllListeners();
        _view._galleryButton.onClick.RemoveAllListeners();
    }


    private void OnCameraButtonClick()
    {
        NativeMethodsManager.OpenCamera(TakePictureCallBack);
    }


    private void TakePictureCallBack(string path)
    {
        Debug.LogError("Camera Image Path :: "+ path);
    }
    
    private void OnGalleryClick()
    {
        NativeMethodsManager.OpenGalleryToPickMedia(TakePictureFromGallery);
    }
    
    private void TakePictureFromGallery(string path)
    {
        Debug.LogError("Gallery Image Path :: "+ path);
    }

}
