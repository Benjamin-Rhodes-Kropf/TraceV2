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
        [SerializeField] private TMP_Text _phoneNumberDisplay;
        [SerializeField] private int lastLength;
        public Button verifyNumberButton;

        private PhoneNumberCanvasController _controller;

        public void editPhoneNumber()
        {
            var phoneNumber = _numberInputField.text;
            _phoneNumberDisplay.text = phoneNumber;

            Debug.Log("PhoneNumberText: modifying phone number:" + _numberInputField.text);

            //first dash in phone number
            if (phoneNumber.Length > 3)
            {
                var phoneNumberVisual = _phoneNumberDisplay.text.Insert(3, "-");
                _phoneNumberDisplay.text = phoneNumberVisual;
            }

            if (phoneNumber.Length > 7)
            {
                var phoneNumberVisual = _phoneNumberDisplay.text.Insert(7, "-");
                _phoneNumberDisplay.text = phoneNumberVisual;
            }

            //second dash in phone number
            // if (phoneNumber.Length == 7 && lastLength != 8)
            // {
            //     var phoneNumberVisual = _inputField.text + "-";
            //     _inputField.text = phoneNumberVisual;
            //     _inputField.MoveToEndOfLine(false, false);
            // }else if (phoneNumber.Length == 7 && lastLength == 8) //if delete key pressed
            // {
            //     _phoneNumberDisplay.text = _inputField.text.Substring(0, _inputField.text.Length - 1);
            // }




            //once at length deselect input field
            if (phoneNumber.Length == 10)
            {
                _numberInputField.DeactivateInputField();
            }

            //keeps phone number length correct
            if (phoneNumber.Length > 10)
            {
                _numberInputField.text = _numberInputField.text.Substring(0, _numberInputField.text.Length - 1);
            }

            lastLength = _numberInputField.text.Length;
        }


    }
}
