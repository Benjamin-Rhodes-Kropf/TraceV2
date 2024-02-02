using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TakePhotoCanvas : MonoBehaviour
{
   public Button _cameraButton;
   public Button _galleryButton;

   private TakePhotoCanvasController _controller;

   private void OnEnable()
   {
      if (_controller is null)
         _controller = new TakePhotoCanvasController(this);
      
      _controller.Init();
   }

   private void OnDisable()
   {
      _controller.Uninit();
   }
}
