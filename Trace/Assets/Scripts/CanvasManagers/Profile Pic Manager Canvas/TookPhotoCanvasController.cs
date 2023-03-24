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
