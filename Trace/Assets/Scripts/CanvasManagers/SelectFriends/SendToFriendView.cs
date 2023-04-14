using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SendToFriendView : MonoBehaviour
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
    public bool sendToThisFriend = false;
    //[SerializeField] private TMP_Text _userName;
    //[SerializeField] private TMP_Text _buttonText;
    //[SerializeField] public Button _addRemoveButton;
    //[SerializeField] private Image _buttonBackground;
    //[SerializeField] private Color[] _colors;

    private string _uid = "";
    public string friendUID {
        get
        {
            return _uid;
        }
    }
    
    

    public void UpdateFrindData(UserModel user, bool isFriendAdd = false)
    {
        //_userName.text = user.Username;
        _nickName.text = user.DisplayName;
        _uid = user.userId;
        FriendView.FriendButtonType buttonType = FriendView.FriendButtonType.Add;
        buttonType = isFriendAdd ? FriendView.FriendButtonType.Remove : FriendView.FriendButtonType.Add;
        //var buttonData = GetButtonData(buttonType);
        //_buttonBackground.color = _colors[buttonData.colorIndex];
        //_buttonText.text = buttonData.buttonText;
        
        //_addRemoveButton.onClick.RemoveAllListeners();
        
        //_addRemoveButton.onClick.AddListener(isFriendAdd ? RemoveFriends :  SendFriendRequest);

        user.ProfilePicture((sprite =>
        {
            try
            {
                _profilePic.texture = sprite.texture;
            }
            catch (Exception e)
            {
                print(e.Message);
            }
        }));
    }

    public void friendPressed()
    {
        if (sendToThisFriend)
        {
            sendToThisFriend = false;
        }
        else
        {
            sendToThisFriend = true;
        }
    }


    private (string buttonText, int colorIndex) GetButtonData(FriendView.FriendButtonType buttonType)
    {
        switch (buttonType)
        {
            case FriendView.FriendButtonType.Add:
                return ("Add", 0);
            case FriendView.FriendButtonType.Remove:
                return ("Remove", 1);
            case FriendView.FriendButtonType.Cancel:
                return ("Cancel", 2);
            default:
                return ("Add", 0);
        }
    }

    public void UpdateRequestStatus(bool RequestSent)
    {
        if (RequestSent)
        {
            FriendView.FriendButtonType buttonType = FriendView.FriendButtonType.Cancel;

            var buttonData = GetButtonData(buttonType);
            //_buttonBackground.color = _colors[buttonData.colorIndex];
            //_buttonText.text = buttonData.buttonText;
        }
    }
    
    
    private void SendFriendRequest()
    {
        //_addRemoveButton.interactable = false;
        string friendUID = this.friendUID;
        
        if (friendUID == "")
            return;

        Debug.LogError("Here after Checking friend ID "+  friendUID);
        if (FriendRequestManager.Instance.IsRequestAllReadyInList(friendUID,false))
            return;
            
        Debug.LogError("Here after Checking List");
        StartCoroutine(FbManager.instance.SendFriendRequest(friendUID,  (IsSuccessful) => {
            if (!IsSuccessful)
            {
                Debug.LogError("Friend request failed at : "+ friendUID);
                return;
            }
            UpdateRequestStatus(true);
            //_addRemoveButton.interactable = true;
            Debug.Log("friend requested at:" + friendUID);
        }));
    }

    private void RemoveFriends()
    {
        // FriendsModelManager.Instance.RemoveFriendFromList(_uid);
        FbManager.instance.RemoveFriends(_uid);
        gameObject.SetActive(false);
    }
}
