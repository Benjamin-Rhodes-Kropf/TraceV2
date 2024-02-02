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
        public Button _backButton;


        private EnterCodeCanvasController _controller;
        private string phoneNumber = "";

        public void Init(string numberStr)
        {
            phoneNumber = numberStr;
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