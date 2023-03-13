using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace CanvasManagers
{
    public class MyFriendCanvas : MonoBehaviour
    {
        public Friend _friendPrefab;
        public TMP_InputField _usernameInput;
        public TMP_Text _numberOfFriendsCount;
        public Transform _displayFrindsParent;
        public List<Friend> _friendsList;

        private FriendCanvasController _controller;


        #region UnityEvents

        private void OnEnable()
        {
            if (_controller == null)
                _controller = new FriendCanvasController();
            
            _controller.Init(this);
        }

        private void OnDisable()
        {
            _controller.UnInitialize();
        }

        #endregion
        
        
        
        
        
    }
}