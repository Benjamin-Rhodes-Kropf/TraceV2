using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SelectFriendViewTrace : MonoBehaviour
{
    [SerializeField] private TMP_Text _profileName;
    [SerializeField] private Image _profilePicture;
    [SerializeField] private Toggle _selectionToggle;


    public void UpdateView(UserModel user)
    {
        _profileName.text = user.DisplayName;
        user.ProfilePicture((sprite =>
        {
            _profilePicture.sprite = sprite;
        }));
        _selectionToggle.isOn = false;
        _selectionToggle.onValueChanged.RemoveAllListeners();
        _selectionToggle.onValueChanged.AddListener(OnToggleClicked);
    }


    private void OnToggleClicked(bool isOn)
    {
        //Todo : Implement This Functionality at end 
    }
    
}
