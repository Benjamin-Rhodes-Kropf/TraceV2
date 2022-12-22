using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PhoneNumberText : MonoBehaviour
{
    [SerializeField] private TMP_InputField _inputField;
    [SerializeField] private TMP_Text _phoneNumberDisplay;
    [SerializeField] private int lastLength;
    public void editPhoneNumber()
    {
        var phoneNumber = _inputField.text;
        _phoneNumberDisplay.text = phoneNumber;
        
        Debug.Log("PhoneNumberText: modifying phone number:" + _inputField.text);

        //first dash in phone number
        if (phoneNumber.Length > 3)
        {
            var phoneNumberVisual = _phoneNumberDisplay.text.Insert(3,"-");
            _phoneNumberDisplay.text = phoneNumberVisual;
        }
        if (phoneNumber.Length > 7)
        {
            var phoneNumberVisual = _phoneNumberDisplay.text.Insert(7,"-");
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
            _inputField.DeactivateInputField();
        }
        
        //keeps phone number length correct
        if (phoneNumber.Length > 10)
        {
            _inputField.text = _inputField.text.Substring(0, _inputField.text.Length - 1);
        }
        
        lastLength = _inputField.text.Length;
    }
}
