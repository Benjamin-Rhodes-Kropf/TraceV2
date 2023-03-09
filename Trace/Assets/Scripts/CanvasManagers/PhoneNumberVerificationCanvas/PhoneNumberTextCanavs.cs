using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CanvasManagers
{
    public class PhoneNumberTextCanavs : MonoBehaviour
    {
        public TMP_InputField _numberInputField;
        public TMP_Dropdown _countryCodeDropdown;
        [SerializeField] public TMP_Text _phoneNumberDisplay;
        [SerializeField] private int lastLength;
        public Button verifyNumberButton;
        public TMP_InputField _countryCodeInputField;
        public EnterCodeCanvas _numberValidationView;
        public GameObject validationScreen;
        private PhoneNumberCanvasController _controller;

        
        #region UnityCallbacks

        private void OnEnable()
        {
            Debug.Log("Register Canvas Enabled");

            if (_controller == null)
                _controller = new PhoneNumberCanvasController(this);
            
            _controller.Init();
            // verifyNumberButton.interactable = false;
        }

        private void OnDisable()
        {
            _controller.Uninitilise();
        }

        #endregion


    }
}
