using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RequestView : MonoBehaviour
{
    public TMP_Text _displayName;
    public TMP_Text _userName;
    public Image _profilePicture;
    public Button _acceptButton;
    public Button _removeButton;


    private string requestId = "";
    private string senderId = "";

    public void UpdateRequestView(UserModel user)
    {
        requestId = FriendRequestManager.Instance.GetRequestID(user.userId);
        senderId = user.userId;
        user.ProfilePicture((sprite =>
        {
            _profilePicture.sprite = sprite;
        }));
        _userName.text = user.Username;
        _displayName.text = user.DisplayName;
        _acceptButton.onClick.RemoveAllListeners();
        _acceptButton.onClick.AddListener(OnClickAccept);
        _removeButton.onClick.RemoveAllListeners();
        _removeButton.onClick.AddListener(OnClickRemove);
    }
  
    public void OnClickAccept()
    {
        print("Accept Function Called");
        StartCoroutine(
        FbManager.instance.AcceptFriendRequest(requestId,senderId,(isUpdated =>
        {
            if (isUpdated)
            {
                FriendRequestManager.Instance.RemoveRequestFromList(senderId);
                this.gameObject.SetActive(false);
            }
        })));
        
    }

    //  TODO: i.  Remove Request From Local List
    //  TODO: ii. Remove Request From Firebase
    public void OnClickRemove()
    {
        FbManager.instance.CancelFriendRequest(requestId);        
        FriendRequestManager.Instance.RemoveRequestFromList(senderId);
        GameObject.Destroy(this);

    }
    
}
