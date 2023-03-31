using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.PackageManager.Requests;
using UnityEngine;
using UnityEngine.UI;

public class RequestView : MonoBehaviour
{
    public TMP_Text _displayName;
    public TMP_Text _userName;
    public Image _profilePicture;
    public Button _acceptButton;
    public Button _removeButton;


    private UserModel user;


    public void UpdateRequestView(UserModel user)
    {
        this.user = user;
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
        
    }


    public void OnClickRemove()
    {
        
    }
    
}
