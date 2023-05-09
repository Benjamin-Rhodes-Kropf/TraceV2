using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using SA.iOS.Contacts;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using UnityEngine.UI;
using VoxelBusters.CoreLibrary;
using VoxelBusters.EssentialKit;

namespace CanvasManagers
{
    public class ContactsCanvasController
    {
        private ContactsCanvas _view;

        private Image _previousSelectedButton;
        private Color32 _selectedButtonColor = new Color32(128, 128, 128, 255);
        private Color32 _unSelectedButtonColor = new Color32(128, 128, 128, 0);
        private List<IAddressBookContact> _allContacts;
        
        private static Regex _compiledUnicodeRegex = new Regex(@"[^\u0000-\u007F]", RegexOptions.Compiled);
        
        public void Init(ContactsCanvas view)
        {
            this._view = view;
            _view._usernameInput.onValueChanged.AddListener(OnInputValueChange);
            
            
            _view._contactsButton.onClick.AddListener(OnContactsSelection);
            _view._friendsButton.onClick.AddListener(OnFriendsSelection);
            _view._requestsButton.onClick.AddListener(OnRequestsSelection);

            OnFriendsSelection();
            UpdateSelectionPanelView();
        }

        public void UnInitialize()
        {
            _view._usernameInput.onValueChanged.RemoveAllListeners();
            
            _view._contactsButton.onClick.RemoveAllListeners();
            _view._friendsButton.onClick.RemoveAllListeners();
            _view._requestsButton.onClick.RemoveAllListeners();
        }


        public void UpdateSelectionPanelView()
        {
            _view._redRequestMark.SetActive(FbManager.instance._allReceivedRequests.Count > 0);
        }
        private void FrindsListInit()
        {
            foreach (var friend in _view._friendsList)
            {
                friend.gameObject.SetActive(false);
            }
        }

        private List<GameObject> searchList;
        private void OnInputValueChange(string inputText)
        {

            if (inputText.Length <= 1)
            {
                ClearSearchList();
                OnFriendsSelection();
            }
            
            var canUpdate = inputText.Length > 1;
            
            if (!canUpdate) return;
            
            inputText = inputText.ToLower();
            ClearSearchList();
            SelectionPanelClick("SearchBar");
            searchList = new List<GameObject>();
            
            UserDataManager.Instance.GetAllUsersBySearch(inputText, out List<UserModel> friends, out List<UserModel> requests,out List<UserModel> requestsSent, out List<UserModel> others);
            TryGetContactsByName(inputText, out List<IAddressBookContact> contacts);
            if (friends.Count > 0)
            {
                var text = GameObject.Instantiate(_view._searchTabTextPrefab, _view._searchscrollParent);
                text.text = "Friends";
                searchList.Add(text.gameObject);
                    
                foreach (var friend in friends)
                {
                    var view = GameObject.Instantiate(_view.friendViewPrefab, _view._searchscrollParent);
                    view.UpdateFriendData(friend,true, FriendsModelManager.Instance.IsBestFriend(friend.userId));
                    searchList.Add(view.gameObject);
                }
            }
            
            if (requests.Count > 0)
            {
                var text = GameObject.Instantiate(_view._searchTabTextPrefab, _view._searchscrollParent);
                text.text = "Requests Received";
                searchList.Add(text.gameObject);
                foreach (var request in requests)
                {
                    var view = GameObject.Instantiate(_view._requestPrefab, _view._searchscrollParent);
                    view.UpdateRequestView(request);
                    searchList.Add(view.gameObject);
                }
            }
            
            if (requestsSent.Count > 0)
            {
                var text = GameObject.Instantiate(_view._searchTabTextPrefab, _view._searchscrollParent);
                text.text = "Requests Sent";
                searchList.Add(text.gameObject);
                foreach (var request in requestsSent)
                {
                    var view = GameObject.Instantiate(_view._requestPrefab, _view._searchscrollParent);
                    view.UpdateRequestView(request,false);
                    searchList.Add(view.gameObject);
                }
            }
            

            if (contacts.Count > 0)
            {
                var text = GameObject.Instantiate(_view._searchTabTextPrefab, _view._searchscrollParent);
                text.text = "Contacts";
                searchList.Add(text.gameObject);
                foreach (var contact in contacts)
                {
                    ContactView view = GameObject.Instantiate(_view._contactPrfab,_view._searchscrollParent);
                    view.UpdateContactInfo(contact);
                    searchList.Add(view.gameObject);
                }
            }
            
            if (others.Count > 0)
            {
                var text = GameObject.Instantiate(_view._searchTabTextPrefab, _view._searchscrollParent);
                text.text = "Others";
                searchList.Add(text.gameObject);
                foreach (var other in others)
                {
                    if (friends.Contains(other)) continue;
                    if (requestsSent.Contains(other)) continue;
                    if (requests.Contains(other)) continue;
                    if (other.userId == FbManager.instance.thisUserModel.userId) continue;
                    var view = GameObject.Instantiate(_view.friendViewPrefab, _view._searchscrollParent);
                    view.UpdateFriendData(other);
                    searchList.Add(view.gameObject);
                }
            }
        }

        private void ClearSearchList()
        {
            if (searchList is not { Count: > 0 })
                return;

            foreach (var ob in searchList)
            {
              GameObject.Destroy(ob);
            } 
        }
        private void ClearFriendList()
        {
            foreach (var friend in _view._friendsList)
            {
                friend.gameObject.SetActive(false);
            }
        }
        
        // TODO: Delete These Functions
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
                        friend.UpdateFriendData(users[userIndex], IsFriendsList);
                        friend.gameObject.SetActive(true);
                        
                    }
                    else
                    {
                        FriendView friend = GameObject.Instantiate(_view.friendViewPrefab, _view._displayFrindsParent);
                        _view._friendsList.Add(friend);
                        friend.UpdateFriendData(users[userIndex], IsFriendsList);
                       
                    }
                }
                else
                {
                    if (userIndex < users.Count)
                    {
                        var friend = _view._friendsList[userIndex];
                        friend.UpdateFriendData(users[userIndex], IsFriendsList);
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
        private void PopulateFriendUIObject(FriendView friendView, UserModel data)
        {
            friendView.UpdateFriendData(data);
            friendView.gameObject.SetActive(true);
            friendView._addRemoveButton.onClick.RemoveAllListeners();
        }


        private List<RequestView> _allRequests;

        public void UpdateRequestLayout()
        {
            if ( _view._requestsScroll.activeInHierarchy)
                LoadAllRequests();
        }
        
        private void OnRequestsSelection()
        {
            LoadAllRequests();
            SelectionPanelClick("Requests");
        }

     
        private void LoadAllRequests()
        {
            var users = UserDataManager.Instance.GetFriendRequested();
            ClearRequestView();
            _allRequests = new List<RequestView>();
            if (users.Count > 0)
            {
                foreach (var user in users)
                    UpdateRequestInfo(user);
            }
            
            var sentRequests = UserDataManager.Instance.GetSentFriendRequests();
            
            if (sentRequests.Count > 0)
            {                
                foreach (var user in sentRequests)
                    UpdateRequestInfo(user, false);
            }
            
            _view._requestText.text = $"Requests ({users.Count + sentRequests.Count})";
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
        private void UpdateRequestInfo(UserModel _user, bool isReceivedRequest = true)
        {
            RequestView view = GameObject.Instantiate(_view._requestPrefab,_view._requestParent);
            view.UpdateRequestView(_user, isReceivedRequest);
            _allRequests.Add(view);
        }

        private List<FriendView> _allFriendsView;

        public void UpdateFriendsLayout()
        {
            if (_view._friendsScroll.activeInHierarchy)
                LoadAllFriends();
        }
        private void OnFriendsSelection()
        {
            LoadAllFriends();
            SelectionPanelClick("Friends");
        }

        private void LoadAllFriends()
        {
            var users = UserDataManager.Instance.GetAllFriends();
            Debug.Log("Update Layout Called");
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
            view.UpdateFriendData(user,true, FriendsModelManager.Instance.IsBestFriend(user.userId));
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
                    _view._searchScroll.SetActive(false);
                    break;
                case "Friends":
                    _previousSelectedButton = _view._friendsButton.GetComponent<Image>();
                    _view._contactsScroll.SetActive(false);
                    _view._friendsScroll.SetActive(true);
                    _view._requestsScroll.SetActive(false);                    
                    _view._searchScroll.SetActive(false);
                    break;
                case "Requests":
                    _previousSelectedButton = _view._requestsButton.GetComponent<Image>();
                    _view._contactsScroll.SetActive(false);
                    _view._friendsScroll.SetActive(false);
                    _view._requestsScroll.SetActive(true);                    
                    _view._searchScroll.SetActive(false);
                    break;
                default:
                    _view._contactsScroll.SetActive(false);
                    _view._friendsScroll.SetActive(false);
                    _view._requestsScroll.SetActive(false);
                    _view._searchScroll.SetActive(true);
                    break;
            }

            _previousSelectedButton.color = _selectedButtonColor;
        }


        private bool isLoaded = false;
        private void OnContactsSelection()
        {
            AddressBookContactsAccessStatus status = AddressBook.GetContactsAccessStatus();
            AddressBook.RequestContactsAccess(callback: OnRequestContactsAccessFinish);
            LoadAllContacts();
            SelectionPanelClick("Contacts");
        }
        private void OnRequestContactsAccessFinish(AddressBookRequestContactsAccessResult result, Error error)
        {
            Debug.Log("Request for contacts access finished.");
            Debug.Log("Address book contacts access status: " + result.AccessStatus);
        }
        
        private void OnReadContactsFinish(AddressBookReadContactsResult result, Error error)
        {
            if (error == null)
            {
                var     contacts    = result.Contacts;
                Debug.Log("Request to read contacts finished successfully.");
                Debug.Log("Total contacts fetched: " + contacts.Length);
                Debug.Log("Below are the contact details (capped to first 10 results only):");
                isLoaded = true;
                foreach (var contact in contacts)
                    LogContactInfo(contact);
            }
            else
            {
                Debug.Log("Request to read contacts failed with error. Error: " + error);
            }
        }
        private void LoadAllContacts()
        {
            if (isLoaded)
                return;

#if UNITY_EDITOR

#elif UNITY_IOS
            _allContacts = new List<IAddressBookContact>();
            AddressBook.ReadContactsWithUserPermission(OnReadContactsFinish);
#endif            
            
        }
        private String StripUnicodeCharactersFromString(string inputValue)
        {
            return _compiledUnicodeRegex.Replace(inputValue, String.Empty);
        }

        private void LogContactInfo(IAddressBookContact contact)
        {
            try
            {
                ContactView view = GameObject.Instantiate(_view._contactPrfab,_view._contactParent);
                view.UpdateContactInfo(contact);
                _allContacts.Add(contact);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        private void TryGetContactsByName(string name, out List<IAddressBookContact> selectedContacts)
        {
            selectedContacts = new List<IAddressBookContact>();
            
            if (_allContacts == null)
                return;
            if (_allContacts.Count <= 0 )
                return;
            // Todo : Check Why Contain Function for Contact Given Name is not working ?
            
            if (string.IsNullOrEmpty(name) is false )
            {
                var list = _allContacts.Where(contact => (contact.FirstName + " "+ contact.LastName).Contains(name, StringComparison.InvariantCultureIgnoreCase)).ToList();
                selectedContacts.AddRange(list);
            }
        }
    }

    public struct Contact
    {
        public string givenName;
        public string phoneNumber;
        public Sprite profile;
    }
}

