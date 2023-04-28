using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectFriendCanvasController
{
    private SelectFriendsCanvas _view;
    private List<UserModel> _allFriends;
    public void Init(SelectFriendsCanvas selectFriendsCanvas)
    {
        _allFriends = UserDataManager.Instance.GetAllFriends();
        _view = selectFriendsCanvas;
        // _view._selectAll.onClick.AddListener(OnSelectAllClicked);
        // _view._deselectAll.onClick.AddListener(OnDeselectAllClicked);
        // _view._continue.onClick.AddListener(OnContinueClicked);
    }

    public void UnInitialize()
    {
        // _view._selectAll.onClick.RemoveAllListeners();
        // _view._deselectAll.onClick.RemoveAllListeners();
        // _view._continue.onClick.RemoveAllListeners();
    }

    private void OnSelectAllClicked()
    {
        // Todo : Implement This Functionality
    }

    private void OnDeselectAllClicked()
    {
        // Todo : Implement This Functionality
    }

    private void OnContinueClicked()
    {
        // Todo : Implement This Functionality
    }
    
}
