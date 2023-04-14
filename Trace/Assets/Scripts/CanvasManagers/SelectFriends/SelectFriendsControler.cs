using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectFriendsControler : MonoBehaviour
{
    private SelectFriendsCanvas _view;
    private List<SendToFriendView> _allFriendsView;
    private List<string> _SendToUIDs;
    public void Init(SelectFriendsCanvas view)
    {
        this._view = view;
        LoadAllFriends();
        _SendToUIDs = new List<string>();
    }
    public void UnInitialize()
    {
        Debug.Log("SelectFriendsControler: UnInitialize()");
    }
    
    private void LoadAllFriends()
    {
        var users = UserDataManager.Instance.GetAllFriends();
        //ClearFriendsView();
        _allFriendsView = new List<SendToFriendView>();
        foreach (var user in users)
        {
            Debug.Log("SelectFriendsControler: LoadAllFriends:" + user.DisplayName);
            UpdateFriendViewInfo(user);
        }
    }
    
    private void FrindsListInit()
    {
        foreach (var friend in _view._friendsList)
        {
            friend.gameObject.SetActive(false);
        }
    }
    private void UpdateFriendViewInfo(UserModel user)
    {
        SendToFriendView view = GameObject.Instantiate(_view.friendViewPrefab, _view._displayFrindsParent);
        view.UpdateFrindData(user,true);
        _allFriendsView.Add(view);
    }

    private void ClearFriendsView()
    {
        if(_allFriendsView.Count <= 0)
            return;
        foreach (var view in _allFriendsView)
            GameObject.Destroy(view.gameObject);
    }
    
    public void UpdateFriendsLayout()
    {
        if (_view._friendsScroll.activeInHierarchy)
            LoadAllFriends();
    }
    
    public void UpdateFriendsSendTo()
    {
        Debug.Log("UpdateFriendsSendTo()");
        foreach (var view in _allFriendsView)
        {
            if (view.sendToThisFriend)
            {
                Debug.Log("UpdateFriendsSendTo:" + view.friendUID);
                _SendToUIDs.Add(view.friendUID);
            }
        }
        
    }

    

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
