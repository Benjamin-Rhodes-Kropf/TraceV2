using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SelectFriendsCanvas : MonoBehaviour{
    
    [Header("Friend Info")]
    public SendToFriendView friendViewPrefab;
    public Transform _displayFrindsParent;
    public List<SendToFriendView> _friendsList;
    public GameObject _friendsScroll;
    
    private SelectFriendsControler _controller;
    
    public static Action UpdateFriendsView;
    public static Action UpdateFriendsSendTo;


    #region UnityEvents

    private void OnEnable()
    {
        if (_friendsList == null)
            _friendsList = new List<SendToFriendView>();
        if (_controller == null)
            _controller = gameObject.AddComponent<SelectFriendsControler>();
        _controller.Init(this);
        UpdateFriendsView += _controller.UpdateFriendsLayout;

        // foreach (var g in GameObject.FindGameObjectsWithTag("UserSelection"))
        // {
        //     Destroy(g);
        // }
    }

    
    
    public void SendButtonPressed()
    {
        Debug.Log("SendButtonPressed()");
        _controller.UpdateFriendsSendTo();
    }
    private void OnDisable()
    {
        ClearFriendsList();
        _controller.UnInitialize();
        UpdateFriendsView -= _controller.UpdateFriendsLayout;
        //UpdateFriendsSendTo -= _controller.UpdateFriendsSendTo;
    }

    public void ClearFriendsList()
    {
        foreach (var obj in _friendsList)
        {
            obj.DestroySelf();
        }
        _friendsList.Clear();
    }
    
    #endregion
    public void TurnOffCamera() {
        ScreenManager.instance.camManager.cameraPanel.SetActive(false);//disabling the camera panel
        ScreenManager.instance.uiController.previewVideoPlayer.gameObject.SetActive(false);//disabling the camera panel
    }
}
