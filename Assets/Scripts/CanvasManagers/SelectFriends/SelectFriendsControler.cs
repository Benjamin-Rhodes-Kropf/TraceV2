using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class SelectFriendsControler : MonoBehaviour
{
    private SelectFriendsCanvas _view;
    private List<SendToFriendView> _allFriendsView;
    public void Init(SelectFriendsCanvas view)
    {
        this._view = view;
        _allFriendsView = new List<SendToFriendView>();
        LoadAllFriends();
    }
    public void UnInitialize()
    {
        _allFriendsView.Clear();
        Debug.Log("SelectFriendsControler: UnInitialize()");
    }
    
    private void LoadAllFriends()
    {
        var users = UserDataManager.Instance.GetAllFriends();
        foreach (var user in users)
        {
            //Debug.Log("SelectFriendsControler: LoadAllFriends:" + user.DisplayName);
            UpdateFriendViewInfo(user);
        }
    }
    
    private void FrindsListInit()
    {
        
    }
    private void UpdateFriendViewInfo(UserModel user)
    {
        SendToFriendView view = GameObject.Instantiate(_view.friendViewPrefab, _view._displayFrindsParent);
        view.UpdateFrindData(user,true);
        _allFriendsView.Add(view);
        _view._friendsList.Add(view);
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
        SendTraceManager.instance.users.Clear();
        Debug.Log("UpdateFriendsSendTo()");
        foreach (var view in _allFriendsView)
        {
            if (view.sendToThisFriend)
            {
                Debug.Log("UpdateFriendsSendTo:" + view.friendUID);
                //_SendToUIDs.Add(view.friendUID);
                SendTraceManager.instance.users.Add(view.friendUID);
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
