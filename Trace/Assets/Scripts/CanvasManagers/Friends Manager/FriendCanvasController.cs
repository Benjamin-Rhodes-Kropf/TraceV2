using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

                    }
                    else
                    {
                         Friend friend = GameObject.Instantiate(_view._friendPrefab,_view._displayFrindsParent);
                         _view._friendsList.Add(friend);
                         friend.UpdateFrindData(users[userIndex]);
                    }
                }
                else
                {
                    if (userIndex < users.Count)
                    {
                        var friend = _view._friendsList[userIndex];
                        friend.UpdateFrindData(users[userIndex]);                        
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
    }
}

