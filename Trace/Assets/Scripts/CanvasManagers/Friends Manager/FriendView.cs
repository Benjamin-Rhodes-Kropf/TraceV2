using System.Collections;
using System.Collections.Generic;
using Networking;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FriendView : MonoBehaviour
{
    public enum FriendButtonType
    {
        Add,
        Remove,
        Cancel,
        Accept
    }

    [SerializeField] private Image _profilePic;
    [SerializeField] private TMP_Text _nickName;
    [SerializeField] private TMP_Text _userName;
    [SerializeField] private TMP_Text _buttonText;
    [SerializeField] public Button _addRemoveButton;
    [SerializeField] private Image _buttonBackground;
    [SerializeField] private Color[] _colors;

    public string Username {
        get
        {
            return _userName.text;
        }
        
    }
    
    

    public void UpdateFrindData(UserModel user)
    {
        _userName.text = user.Username;
        _nickName.text = user.DisplayName;

        FriendButtonType buttonType = FriendButtonType.Add;

        var buttonData = GetButtonData(buttonType);
        _buttonBackground.color = _colors[buttonData.colorIndex];
        _buttonText.text = buttonData.buttonText;
        
        DownloadHandler.Instance.DownloadImage(user.PhotoURL, (texture) =>
        {
            
        });

    }


    private (string buttonText, int colorIndex) GetButtonData(FriendButtonType buttonType)
    {
        switch (buttonType)
        {
            case FriendButtonType.Add:
                return ("Add", 0);
            case FriendButtonType.Remove:
                return ("Remove", 1);
            default:
                return ("Add", 0);
        }
    }
}