using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CanvasManagers
{
    public class EnterCodeCanvas : MonoBehaviour
    {
        public TextMeshProUGUI _phoneNumberPreviewText;
        public TMP_InputField _verificationCode;
        public Button _submitButton;
        public Button _requestNewCodeButton;


        private EnterCodeCanvasController _controller;



        public void OnEditVerificationCode()
        {
            var vCode = _verificationCode.text;

            if (vCode.Length < 6)
            {
                _submitButton.interactable = false;
            }
            
            if (vCode.Length == 6)
            {
                _verificationCode.DeactivateInputField();
                _submitButton.interactable = true;
            }

            if (vCode.Length > 6)
            {
                _verificationCode.text = _verificationCode.text.Substring(0, _verificationCode.text.Length - 1);
            }
            
        }
        
        

        private void OnEnable()
        {
            if (_controller is null)
                _controller = new EnterCodeCanvasController(this);
            
            _submitButton.interactable = false;
            _controller.Init();
        }


        private void OnDisable()
        {
            if (_controller != null)
                _controller.UnInit();
        }
    }
}