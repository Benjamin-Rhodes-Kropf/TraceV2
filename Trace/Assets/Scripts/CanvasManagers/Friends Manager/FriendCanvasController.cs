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
        }

        public void UnInitialize()
        {
            _view._usernameInput.onValueChanged.RemoveAllListeners();
        }




        private void OnInputValueChange(string inputText)
        {
           bool shouldUpdate = inputText.Length > 2;
        }
        
        
    }
}