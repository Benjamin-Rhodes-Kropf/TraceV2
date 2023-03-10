using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisplayNameCanvasController
{
    private DisplayNameCanvas _view;
    

    public DisplayNameCanvasController(DisplayNameCanvas instance)
    {
        _view = instance;
    }


    public void Init()
    {
        _view._submitButton.onClick.AddListener(OnSubmitButtonClick);
    }

    public void Uninitialise()
    {
        _view._submitButton.onClick.RemoveAllListeners();
        
    }


    public void OnSubmitButtonClick()
    {
        if (_view._displayNameInputField.text == "")
            return;

        var name = _view._displayNameInputField.text;

        _view.StartCoroutine(FbManager.instance.SetUserNickName(name, (isSuccess) =>
        {
            if (isSuccess)
                ScreenManager.instance.ChangeScreenForwards("TakePhoto");
            else
                Debug.LogError("Name Is Not Updated");
        }));
    }
}
