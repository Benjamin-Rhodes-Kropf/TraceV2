using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TookPhotoCanvasController
{
    private TookPhotoCanvas _view;



    public TookPhotoCanvasController(TookPhotoCanvas view)
    {
        this._view = view;
    }

    public void Init()
    {
        _view._doneButton.onClick.AddListener(OnDoneButtonClick);
        _view._moveBack.onClick.AddListener(OnMoveBackClick);
        LoadImageFromPath();
    }
    
    private void LoadImageFromPath()
    {
        Texture2D tex = new Texture2D(2, 2);
        byte[] imageBytes = System.IO.File.ReadAllBytes(TakePhotoCanvasController.imagePath);
        tex.LoadImage(imageBytes);
        var sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(tex.width/2, tex.height/2));
        _view._profilePicture.sprite = sprite;
    }
    
    public void Uninit()
    {
        _view._doneButton.onClick.RemoveAllListeners();
        _view._moveBack.onClick.RemoveAllListeners();
    }
    
    private void OnDoneButtonClick()
    {
        _view.StartCoroutine(FbManager.instance.SetUserProfilePhotoUrl(TakePhotoCanvasController.photoUrl,
            (isSuccess) =>
            {
                if (isSuccess)
                {
                    ScreenManager.instance.ChangeScreenForwards("SettingUpAccount");
                }
            }));
    }


    private void OnMoveBackClick()
    {
        ScreenManager.instance.ChangeScreenForwards("TakePhoto");
    }
  
}
