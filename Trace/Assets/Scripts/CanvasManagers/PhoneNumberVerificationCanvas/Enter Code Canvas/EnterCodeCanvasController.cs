using System.Collections;
using System.Collections.Generic;
using System.Drawing.Printing;
using UnityEngine;
using UnityEngine.UI;

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
            this._view._requestNewCodeButton.onClick.AddListener(OnRequestNewCodeClick);
            this._view._verificationCode.onValueChanged.AddListener(OnEditVerificationCode);
            this._view._backButton.onClick.AddListener(OnBackButtonClick);

        }


        public void UnInit()
        {
            this._view._requestNewCodeButton.onClick.RemoveAllListeners();
            this._view._verificationCode.onValueChanged.RemoveAllListeners();
            this._view._backButton.onClick.RemoveAllListeners();
        }

        public void OnBackButtonClick()
        {
            this._view.gameObject.SetActive(false);
        }

        public void OnRequestNewCodeClick()
        {
        }

        public void OnEditVerificationCode(string inputText)
        {
            var vCode = inputText;

            if (vCode.Length < 6)
            {
                _view._submitButton.interactable = false;
            }

            if (vCode.Length == 6)
            {
                _view._verificationCode.DeactivateInputField();
                _view._submitButton.interactable = true;
            }

            if (vCode.Length > 6)
            {
                _view._verificationCode.text =
                    _view._verificationCode.text.Substring(0, _view._verificationCode.text.Length - 1);
            }
        }
    }
}