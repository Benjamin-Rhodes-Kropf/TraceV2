using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

namespace CanvasManagers
{
    public class ContactsCanvasController
    {
        private ContactsCanvas _view;
        

        public void Init(ContactsCanvas view)
        {
            this._view = view;
            _view._usernameInput.onValueChanged.AddListener(OnInputValueChange);
            FrindsListInit();

            _view._numberOfFriendsCountTitle.text = "0 Friends";
            _view._numberOfFriendsCountScroll.text = "My Friends (0)";
        }

        public void UnInitialize()
        {
            _view._usernameInput.onValueChanged.RemoveAllListeners();
        }

        private void FrindsListInit()
        {
            foreach (var friend in _view._friendsList)
            {
                friend.gameObject.SetActive(false);
            }
        }


        private void SendFriendRequest(FriendView friend)
        {
            string username = friend.Username;
            // username = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject.transform
            //     .GetComponentInParent<FriendView>().Username;
            
            
            if (username == "")
            {
                //Todo: update visuals accordingly
                //return with smth along the lines of "you have to search a valid user first"
                return;
            }
            //else make friend request
            _view.StartCoroutine(FbManager.instance.ActionFriendRequest(username,  (callbackObject) => {
                if (!callbackObject.IsSuccessful)
                {
                    Debug.LogError("Friend request failed at : "+ username);
                    //todo: make visual for non-valid return
                    return;
                }
                //todo: make visual for valid return
                friend.UpdateRequestStatus(true);
                Debug.Log("friend requested at:" + username);
            }));
        }
        
        // public void SendFriendRequest(string friendId)
        // {
        //     // Send push notification to friend
        //     string title = "New Friend Request";
        //     string body = "You have a new friend request from " + friendId;
        //     string topic = friendId; // Send the notification to the friend's device
        //
        //     // FirebaseMessage message = new FirebaseMessage();
        //
        // }
        
        private void OnInputValueChange(string inputText)
        {

            if (inputText.Length <= 0)
                PopulateFriendsList(0,null);
            
            var canUpdate = inputText.Length > 2;
            
            if (!canUpdate) return;
            
            inputText = inputText.ToLower();

            var users = UserDataManager.Instance.GetUsersByLetters(inputText);

            if (users  ==  null)
            {
                Debug.LogError("No Users Exist");
                return;
            }

            int allUsersCount = users.Count;
            PopulateFriendsList(allUsersCount, users);
        }

        // TODO: Need to refactor this method
        private void PopulateFriendsList(int allUsersCount, List<UserModel> users)
        {
            int allFrindsTileCount = _view._friendsList.Count;

            bool isNeedToAddMoreTiles = allUsersCount > allFrindsTileCount;

            int totalUsers = isNeedToAddMoreTiles ? allUsersCount : allFrindsTileCount;

            for (int userIndex = 0; userIndex < totalUsers; userIndex++)
            {
                if (isNeedToAddMoreTiles)
                {
                    if (userIndex < _view._friendsList.Count)
                    {
                        var friend = _view._friendsList[userIndex];
                        friend.UpdateFrindData(users[userIndex]);
                        friend.gameObject.SetActive(true);
                        friend._addRemoveButton.onClick.RemoveAllListeners();
                        friend._addRemoveButton.onClick.AddListener(() =>
                        {
                            SendFriendRequest(friend);
                        });
                    }
                    else
                    {
                        FriendView friend = GameObject.Instantiate(_view.friendViewPrefab, _view._displayFrindsParent);
                        _view._friendsList.Add(friend);
                        friend.UpdateFrindData(users[userIndex]);
                        friend._addRemoveButton.onClick.RemoveAllListeners();
                        friend._addRemoveButton.onClick.AddListener(() =>
                        {
                            SendFriendRequest(friend);
                        });
                    }
                }
                else
                {
                    if (userIndex < users.Count)
                    {
                        var friend = _view._friendsList[userIndex];
                        friend.UpdateFrindData(users[userIndex]);
                        friend.gameObject.SetActive(true);
                        friend._addRemoveButton.onClick.RemoveAllListeners();
                        friend._addRemoveButton.onClick.AddListener(() =>
                        {
                            SendFriendRequest(friend);
                        });
                    }
                    else
                    {
                        var friend = _view._friendsList[userIndex];
                        friend.gameObject.SetActive(false);
                        friend._addRemoveButton.onClick.RemoveAllListeners();
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
    }
}

