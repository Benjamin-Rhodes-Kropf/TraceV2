using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisplayNameCanvasController
{
    private DisplayNameCanvas _view;
    private bool _isUsernameValidated = false;


    public DisplayNameCanvasController(DisplayNameCanvas instance)
    {
        _view = instance;
    }


    public void Init()
    {
        _view._username.onEndEdit.AddListener(ValidateUsername);
        _view._submitButton.onClick.AddListener(OnSubmitButtonClick);
    }

    public void Uninitialise()
    {
        _view._submitButton.onClick.RemoveAllListeners();
        _view._username.onEndEdit.RemoveAllListeners();
        EnableUserNameCheckButton();
    }


    public void OnSubmitButtonClick()
    {
        if (_view._displayNameInputField.text == "")
        {
            _view.ShowMessage("Enter Display Message");
            return;
        }

        var name = _view._displayNameInputField.text;
        var username = _view._username.text.ToLower();
        
        _view.StartCoroutine(FbManager.instance.SetUserNickName(name, (isSuccess) =>
        {
            if (!isSuccess)
                Debug.LogError("Display Name Is Not Updated");
        }));

        _view.StartCoroutine(FbManager.instance.SetUsername(username, (isSuccess) =>
        {
            if (isSuccess)
                ScreenManager.instance.ChangeScreenForwards("TakePhoto");
            else
                Debug.LogError("username Is Not Updated");
        }));
    }


    private void ValidateUsername(string username)
    {
        username = username.ToLower();
        _isUsernameValidated = !HelperMethods.isBadName(username);

        if (_isUsernameValidated)
        {
            bool isUsernameAvailable = UserDataManager.Instance.IsUsernameAvailable(username);
            if (isUsernameAvailable  is false)
            {
                _view.ShowMessage("This username is not available to use !");
                _isUsernameValidated = false;
            }
        }
        
        EnableUserNameCheckButton();
    }


    public void EnableUserNameCheckButton()
    { 
        _view._submitButton.interactable = _isUsernameValidated;
    }
}
