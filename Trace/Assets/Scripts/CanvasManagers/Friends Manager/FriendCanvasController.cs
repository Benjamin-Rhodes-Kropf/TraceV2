using System.Collections;
using System.Collections.Generic;
using Firebase.Messaging;
using UnityEngine;
using UnityEngine.Playables;

namespace CanvasManagers
{
    public class FriendCanvasController
    {
        private MyFriendCanvas _view;
        

        public void Init(MyFriendCanvas view)
        {
            this._view = view;
            _view._usernameInput.onValueChanged.AddListener(OnInputValueChange);
            FrindsListInit();
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


        private void SendFriendRequest()
        {
            string username = "";
            username = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject.transform
                .GetComponentInParent<Friend>().Username;
            
            
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
                    Debug.LogError("Friend Requested at : "+ username);
                    //todo: make visual for non-valid return
                    return;
                }
                //todo: make visual for valid return
                Debug.Log("friend requested at:" + username);
            }));
        }
        
        public void SendFriendRequest(string friendId)
        {
            // Send push notification to friend
            string title = "New Friend Request";
            string body = "You have a new friend request from " + friendId;
            string topic = friendId; // Send the notification to the friend's device

            FirebaseMessage message = new FirebaseMessage();

        }
        
        private void OnInputValueChange(string inputText)
        {
            var canUpdate = inputText.Length > 2;

            if (!canUpdate) return;

            var users = UserDataManager.Instance.GetUsersByLetters(inputText);

            if (users  ==  null)
            {
                Debug.LogError("No Users Exist");
                return;
            }

            int allUsersCount = users.Count;
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
                        friend._addRemoveButton.onClick.AddListener(SendFriendRequest);

                    }
                    else
                    {
                         Friend friend = GameObject.Instantiate(_view._friendPrefab,_view._displayFrindsParent);
                         _view._friendsList.Add(friend);
                         friend.UpdateFrindData(users[userIndex]);
                         friend._addRemoveButton.onClick.RemoveAllListeners();
                         friend._addRemoveButton.onClick.AddListener(SendFriendRequest);
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
                        friend._addRemoveButton.onClick.AddListener(SendFriendRequest);

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
    }
}

