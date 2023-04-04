using System;
using System.Collections;
using System.Collections.Generic;
using SA.iOS.Contacts;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;

namespace CanvasManagers
{
    public class ContactsCanvasController
    {
        private ContactsCanvas _view;

        private Image _previousSelectedButton;
        private Color32 _selectedButtonColor = new Color32(128, 128, 128, 255);
        private Color32 _unSelectedButtonColor = new Color32(128, 128, 128, 0);
        
        public void Init(ContactsCanvas view)
        {
            this._view = view;
            _view._usernameInput.onValueChanged.AddListener(OnInputValueChange);
            
            
            _view._contactsButton.onClick.AddListener(OnContactsSelection);
            _view._friendsButton.onClick.AddListener(OnFriendsSelection);
            _view._requestsButton.onClick.AddListener(OnRequestsSelection);

            OnFriendsSelection();
            
            FrindsListInit();
        }

        public void UnInitialize()
        {
            _view._usernameInput.onValueChanged.RemoveAllListeners();
            
            _view._contactsButton.onClick.RemoveAllListeners();
            _view._friendsButton.onClick.RemoveAllListeners();
            _view._requestsButton.onClick.RemoveAllListeners();
        }

        private void FrindsListInit()
        {
            foreach (var friend in _view._friendsList)
            {
                friend.gameObject.SetActive(false);
            }
        }
        private void OnInputValueChange(string inputText)
        {

            if (inputText.Length <= 0)
                ClearFriendList();
            
            var canUpdate = inputText.Length > 2;
            
            if (!canUpdate) return;
            
            inputText = inputText.ToLower();

            var users = UserDataManager.Instance.GetUsersByLetters(inputText);

            if (users  ==  null)
            {
                Debug.LogError("No Users Exist");
                return;
            }
        }

        private void ClearFriendList()
        {
            foreach (var friend in _view._friendsList)
            {
                friend.gameObject.SetActive(false);
            }
        }
        
        
        // TODO: Need to refactor this method
        private void PopulateFriendsList(List<UserModel> users, bool IsFriendsList = false)
        {
            int allFrindsTileCount = _view._friendsList.Count;
            int allUsersCount = users.Count;
            bool isNeedToAddMoreTiles = allUsersCount > allFrindsTileCount;

            int totalUsers = isNeedToAddMoreTiles ? allUsersCount : allFrindsTileCount;

            for (int userIndex = 0; userIndex < totalUsers; userIndex++)
            {
                if (isNeedToAddMoreTiles)
                {
                    if (userIndex < _view._friendsList.Count)
                    {
                        var friend = _view._friendsList[userIndex];
                        friend.UpdateFrindData(users[userIndex], IsFriendsList);
                        friend.gameObject.SetActive(true);
                        
                    }
                    else
                    {
                        FriendView friend = GameObject.Instantiate(_view.friendViewPrefab, _view._displayFrindsParent);
                        _view._friendsList.Add(friend);
                        friend.UpdateFrindData(users[userIndex], IsFriendsList);
                       
                    }
                }
                else
                {
                    if (userIndex < users.Count)
                    {
                        var friend = _view._friendsList[userIndex];
                        friend.UpdateFrindData(users[userIndex], IsFriendsList);
                        friend.gameObject.SetActive(true);
                        
                    }
                    else
                    {
                        var friend = _view._friendsList[userIndex];
                        friend.gameObject.SetActive(false);
                    }
                }
            }
        }

        // TODO: Refactor it later
        private void PopulateFriendUIObject(FriendView friendView, UserModel data)
        {
            friendView.UpdateFrindData(data);
            friendView.gameObject.SetActive(true);
            friendView._addRemoveButton.onClick.RemoveAllListeners();
            // friendView._addRemoveButton.onClick.AddListener(SendFriendRequest);
        }


        private List<RequestView> _allRequests;
        private void OnRequestsSelection()
        {
            LoadAllRequests();
            SelectionPanelClick("Requests");
        }

        private void LoadAllRequests()
        {
            var users = UserDataManager.Instance.GetFriendRequested();
            _view._requestText.text = $"Requests ({users.Count})";
            ClearRequestView();
            _allRequests = new List<RequestView>();
            foreach (var user in users)
                UpdateRequestInfo(user);
        }

        private void ClearRequestView()
        {
            if (_allRequests == null)
                return;
            if (_allRequests.Count <= 0)
                return;
            foreach (var request in _allRequests)
                GameObject.Destroy(request.gameObject);
        }
        private void UpdateRequestInfo(UserModel _user)
        {
            RequestView view = GameObject.Instantiate(_view._requestPrefab,_view._requestParent);
            view.UpdateRequestView(_user);
            _allRequests.Add(view);
        }

        private List<FriendView> _allFriendsView;
        private void OnFriendsSelection()
        {
            LoadAllFriends();
            SelectionPanelClick("Friends");
        }

        private void LoadAllFriends()
        {
            var users = UserDataManager.Instance.GetAllFriends();
            
            ClearFriendsView();
            
            _view._numberOfFriendsCountTitle.text = $"{users.Count} Friends";
            _view._numberOfFriendsCountScroll.text = $"My Friends ({users.Count})";
            _allFriendsView = new List<FriendView>();
            foreach (var user in users)
                UpdateFriendViewInfo(user);
        }


        private void UpdateFriendViewInfo(UserModel user)
        {
            FriendView view = GameObject.Instantiate(_view.friendViewPrefab, _view._displayFrindsParent);
            view.UpdateFrindData(user,true);
            _allFriendsView.Add(view);
        }
        private void ClearFriendsView()
        {
            if (_allFriendsView == null)
            {
                _allRequests = new List<RequestView>();
                return;
            }
            
            if(_allFriendsView.Count <= 0)
                return;
            foreach (var view in _allFriendsView)
                GameObject.Destroy(view.gameObject);
        }

        private void SelectionPanelClick(string _selectedButton)
        {
            if (_previousSelectedButton != null)
                _previousSelectedButton.color = _unSelectedButtonColor;
            switch (_selectedButton)
            {
                case "Contacts":
                    _previousSelectedButton = _view._contactsButton.GetComponent<Image>();
                    _view._contactsScroll.SetActive(true);
                    _view._friendsScroll.SetActive(false);
                    _view._requestsScroll.SetActive(false);
                    break;
                case "Friends":
                    _previousSelectedButton = _view._friendsButton.GetComponent<Image>();
                    _view._contactsScroll.SetActive(false);
                    _view._friendsScroll.SetActive(true);
                    _view._requestsScroll.SetActive(false);
                    break;
                case "Requests":
                    _previousSelectedButton = _view._requestsButton.GetComponent<Image>();
                    _view._contactsScroll.SetActive(false);
                    _view._friendsScroll.SetActive(false);
                    _view._requestsScroll.SetActive(true);
                    break;
                default:
                    break;
            }

            _previousSelectedButton.color = _selectedButtonColor;
        }


        private bool isLoaded = false;
        private void OnContactsSelection()
        {
            LoadAllContacts();
            SelectionPanelClick("Contacts");
        }
        
        private void LoadAllContacts()
        {
            if (isLoaded)
                return;
#if UNITY_EDITOR
           
#elif UNITY_IOS
             ISN_CNContactStore.FetchPhoneContacts((result) => {
                if(result.IsSucceeded)
                {
                    isLoaded = true;
                    foreach (var contact in result.Contacts)
                        LogContactInfo(contact);
                } 
                else 
                    Debug.Log("Error: " + result.Error.Message);
            });
#endif            
            
        }
        private void LogContactInfo(ISN_CNContact contact)
        {
            try
            {
                ContactView view = GameObject.Instantiate(_view._contactPrfab,_view._contactParent);
                view.UpdateContactInfo(contact.GivenName,contact.Phones[0].FullNumber);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }

}

