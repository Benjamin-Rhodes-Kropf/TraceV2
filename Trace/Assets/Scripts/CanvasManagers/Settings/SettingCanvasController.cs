using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingCanvasController
{
    private SettingsCanvas _view;
    
    
    public void Init(SettingsCanvas settingsCanvas)
    {
        _view = settingsCanvas;
        UpdateDate();
    }

    public void UnInitialize()
    {
        
    }

    private void UpdateDate()
    {
        _view._usernameText.text = FbManager.instance._currentUser.Username;
        _view._profileNameText.text = FbManager.instance._currentUser.DisplayName;
        FbManager.instance._currentUser.ProfilePicture(sprite =>
        {
            _view._profileImage.sprite = sprite;
        });
    }

   
}
