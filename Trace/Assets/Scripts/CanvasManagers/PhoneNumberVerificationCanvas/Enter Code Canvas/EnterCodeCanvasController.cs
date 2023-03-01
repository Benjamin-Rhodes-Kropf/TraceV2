using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using  UnityEngine.UI;

namespace CanvasManagers
{
    public class EnterCodeCanvasController
    {
        private EnterCodeCanvas _view;


        public EnterCodeCanvasController(EnterCodeCanvas _view)
        {
            this._view = _view;
        }


        public void Init()
        {
            this._view._submitButton.onClick.AddListener(OnSubmitButtonClick);
            this._view._requestNewCodeButton.onClick.AddListener(OnRequestNewCodeClick);
        }


        public void UnInit()
        {
            this._view._submitButton.onClick.RemoveAllListeners();
            this._view._requestNewCodeButton.onClick.RemoveAllListeners();
        }


        public void OnSubmitButtonClick()
        {
            
        }

        public void OnRequestNewCodeClick()
        {
            
        }
    }
}