using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class FriendsSearchPopup : MonoBehaviour
{
    [SerializeField] private TMP_InputField friendSearchBar;
    [SerializeField] private string searchedUserID;
    [SerializeField] private Texture searchedUserProfilePhoto;
    [SerializeField] private RawImage displayUserPhoto;
    void OnDisable()
    {
        searchedUserID = "";
        Debug.Log("FriendsSearchPopup: script was disabled");
    }

    void OnEnable()
    {
        searchedUserID = "";
        Debug.Log("FriendsSearchPopup: script was enabled");
        friendSearchBar.text = "";
    }
    
    public void SearchForUser()
    {
        StartCoroutine(FbManager.instance.SearchForUserByUsername(friendSearchBar.text, (myReturnValue) => {
            if (myReturnValue.IsSuccessful)
            {
                Debug.Log("search returned user");
                Debug.Log(myReturnValue.ReturnValue);
                searchedUserID = myReturnValue.message.ToString();
                searchedUserProfilePhoto = (Texture2D)myReturnValue.ReturnValue;
                displayUserPhoto.texture = searchedUserProfilePhoto;
            }
            else
            {
                Debug.LogWarning("failed to find user");
            }
        }));
    }
    
    public void RequestFriendWithSearchedUser()
    {
        if (searchedUserID == "")
        {
            //Todo: update visuals accordingly
            //return with smth along the lines of "you have to search a valid user first"
            return;
        }
        //else make friend request
        StartCoroutine(FbManager.instance.ActionFriendRequest(searchedUserID,  (callbackObject) => {
            if (!callbackObject.IsSuccessful)
            {
                //todo: make visual for non-valid return
                return;
            }
            //todo: make visual for valid return
            Debug.Log("friend requested at:" + searchedUserID);
        }));
    }
}
