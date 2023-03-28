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

    [SerializeField] private RawImage _profilePic;
    [SerializeField] private TMP_Text _nickName;
    [SerializeField] private TMP_Text _userName;
    [SerializeField] private TMP_Text _buttonText;
    [SerializeField] public Button _addRemoveButton;
    [SerializeField] private Image _buttonBackground;
    [SerializeField] private Color[] _colors;

    private string _uid = "";
    public string friendUID {
        get
        {
            return _uid;
        }
        
    }
    
    

    public void UpdateFrindData(UserModel user)
    {
        _userName.text = user.Username;
        _nickName.text = user.DisplayName;
        _uid = user.userId;
        FriendButtonType buttonType = FriendButtonType.Add;

        var buttonData = GetButtonData(buttonType);
        _buttonBackground.color = _colors[buttonData.colorIndex];
        _buttonText.text = buttonData.buttonText;
        
        FbManager.instance.GetProfilePhotoFromFirebaseStorage(user.userId, (texture) =>
        {
            _profilePic.texture = texture;
        });
        
        
        // DownloadHandler.Instance.DownloadImage(user.PhotoURL, (texture) =>
        // {
        //     _profilePic.texture = texture;
        // }, () =>
        // {
        //     Debug.Log("Error");
        // });

    }


    private (string buttonText, int colorIndex) GetButtonData(FriendButtonType buttonType)
    {
        switch (buttonType)
        {
            case FriendButtonType.Add:
                return ("Add", 0);
            case FriendButtonType.Remove:
                return ("Remove", 1);
            case FriendButtonType.Cancel:
                return ("Cancel", 2);
            default:
                return ("Add", 0);
        }
    }

    public void UpdateRequestStatus(bool RequestSent)
    {
        if (RequestSent)
        {
            FriendButtonType buttonType = FriendButtonType.Cancel;

            var buttonData = GetButtonData(buttonType);
            _buttonBackground.color = _colors[buttonData.colorIndex];
            _buttonText.text = buttonData.buttonText;
        }
    }
}