using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectFriendsControler : MonoBehaviour
{
    private SelectFriendsCanvas _view;
    private List<SendToFriendView> _allFriendsView;
    
    public void Init(SelectFriendsCanvas view)
    {
        this._view = view;
        
        LoadAllFriends();
    }
    public void UnInitialize()
    {
        Debug.Log("SelectFriendsControler: UnInitialize()");
    }
    
    private void LoadAllFriends()
    {
        var users = UserDataManager.Instance.GetAllFriends();
        Debug.LogError("Update Layout Called");
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

    

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
