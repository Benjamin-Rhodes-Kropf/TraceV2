using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TookPhotoCanvasController
{
    private TookPhotoCanvas _view;

    public static Sprite _profilePicture;

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
        if (TakePhotoCanvasController.imagePath == "")
            return;
        
        Texture2D tex = new Texture2D(2, 2);
        byte[] imageBytes = System.IO.File.ReadAllBytes(TakePhotoCanvasController.imagePath);
        tex.LoadImage(imageBytes);
        _profilePicture = CropTexture(tex);
        _view._profilePicture.sprite = _profilePicture;
    }

    private Sprite CropTexture(Texture2D texture)
    {
        Sprite croppedSprite = null;
        Texture2D originalTexture = texture;
        int squareSize = Mathf.Min(originalTexture.width, originalTexture.height);
        Rect croppingRect = new Rect((originalTexture.width - squareSize) / 2, (originalTexture.height - squareSize) / 2, squareSize, squareSize);
        Texture2D croppedTexture = new Texture2D((int)croppingRect.width, (int)croppingRect.height);
        croppedTexture.SetPixels(originalTexture.GetPixels((int)croppingRect.x, (int)croppingRect.y, (int)croppingRect.width, (int)croppingRect.height));
        croppedTexture.Apply();
        croppedSprite = Sprite.Create(croppedTexture, new Rect(0, 0, croppedTexture.width, croppedTexture.height), new Vector2(0.5f, 0.5f));
        return croppedSprite;
    }
    
    public void Uninit()
    {
        _view._doneButton.onClick.RemoveAllListeners();
        _view._moveBack.onClick.RemoveAllListeners();
    }
    
    private void OnDoneButtonClick()
    {
                    ScreenManager.instance.ChangeScreenForwards("SettingUpAccount");
        
    }


    private void OnMoveBackClick()
    {
        ScreenManager.instance.ChangeScreenForwards("TakePhoto");
    }
  
}
