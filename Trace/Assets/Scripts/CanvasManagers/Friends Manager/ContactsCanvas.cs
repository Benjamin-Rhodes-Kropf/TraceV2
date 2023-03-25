using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace CanvasManagers
{
    public class ContactsCanvas : MonoBehaviour
    {
        public Friend _friendPrefab;
        public TMP_InputField _usernameInput;
        public TMP_Text _numberOfFriendsCount;
        public Transform _displayFrindsParent;
        public List<Friend> _friendsList;

        private ContactsCanvasController _controller;


        #region UnityEvents

        private void OnEnable()
        {
            if (_controller == null)
                _controller = new ContactsCanvasController();
            
            _controller.Init(this);
        }

        private void OnDisable()
        {
            _controller.UnInitialize();
        }

        #endregion
        
        
        
        
        
    }
}