using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace CanvasManagers
{
    public class ContactsCanvas : MonoBehaviour
    {
        [FormerlySerializedAs("_friendPrefab")] public FriendView friendViewPrefab;
        public TMP_InputField _usernameInput;
        public TMP_Text _numberOfFriendsCountTitle;
        public TMP_Text _numberOfFriendsCountScroll;
        public Transform _displayFrindsParent;
        public List<FriendView> _friendsList;

        [Header("Contacts Info")] 
        public ContactView _contactPrfab;
        public Transform _contactParent;



        [Header("Toggle Panels")] 
        public GameObject _contactsScroll;
        public GameObject _friendsScroll;
        public GameObject _requestsScroll;
        public GameObject _searchScroll;


        [Header("Panel Toggle Buttons")] 
        public Button _contactsButton;
        public Button _friendsButton;
        public Button _requestsButton;
        
        
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