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
        if (_controller == null)
            _controller = new SelectFriendsControler();
        _controller.Init(this);
        UpdateFriendsView += _controller.UpdateFriendsLayout;
        //UpdateFriendsSendTo += _controller.UpdateFriendsSendTo;
    }
    
    public void SendButtonPressed()
    {
        Debug.Log("SendButtonPressed()");
        _controller.UpdateFriendsSendTo();
    }

    private void OnDisable()
    {
        _controller.UnInitialize();
        UpdateFriendsView -= _controller.UpdateFriendsLayout;
        //UpdateFriendsSendTo -= _controller.UpdateFriendsSendTo;
    }
    
    #endregion
    public void TurnOffCamera() {
        ScreenManager.instance.camManager.cameraPanel.SetActive(false);//disabling the camera panel
        ScreenManager.instance.uiController.previewVideoPlayer.gameObject.SetActive(false);//disabling the camera panel
    }
}
