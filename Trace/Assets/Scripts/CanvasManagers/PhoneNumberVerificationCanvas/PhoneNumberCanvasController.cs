using Firebase.Auth;
using UnityEngine;

namespace CanvasManagers
{
    public class PhoneNumberCanvasController
    {
        private PhoneNumberTextCanavs _view;
        
        public PhoneNumberCanvasController(PhoneNumberTextCanavs canvas)
        {
            _view = canvas;
        }

        public void Init()
        {
            _view.verifyNumberButton.onClick.AddListener(OnVerifyNumberClicked);
            _view._numberInputField.onValueChanged.AddListener(EditPhoneNumber);
        }

        public void Uninitilise()
        {
            _view.verifyNumberButton.onClick.RemoveAllListeners();
            _view._numberInputField.onValueChanged.RemoveAllListeners();

        }

        private void OnVerifyNumberClicked()
        {
            string phoneNumber = _view._countryCodeInputField.text.Trim()+_view._numberInputField.text.Trim();
            
            PhoneAuthProvider provider = PhoneAuthProvider.GetInstance(FirebaseAuth.DefaultInstance);
            
            provider.VerifyPhoneNumber(phoneNumber, 30, null,
                verificationCompleted: (credential) => {
                    Debug.LogError(credential.Provider);
                    // Auto-sms-retrieval or instant validation has succeeded (Android only).
                    // There is no need to input the verification code.
                    // `credential` can be used instead of calling GetCredential().
                },
                verificationFailed: (error) => {
                    // The verification code was not sent.
                    Debug.LogError(error);

                    // `error` contains a human readable explanation of the problem.
                },
                codeSent: (id, token) => {
                    Debug.LogError(id);
                    Debug.LogError(token);

                    // Verification code was successfully sent via SMS.
                    // `id` contains the verification id that will need to passed in with
                    // the code from the user when calling GetCredential().
                    // `token` can be used if the user requests the code be sent again, to
                    // tie the two requests together.
                },
                codeAutoRetrievalTimeOut: (id) => {
                    Debug.LogError(id);

                    // Called when the auto-sms-retrieval has timed out, based on the given
                    // timeout parameter.
                    // `id` contains the verification id of the request that timed out.
                });

            ActiveValidationWindow(phoneNumber);
        }
        
        
        private void ActiveValidationWindow(string number)
        {
            _view.validationScreen.SetActive(true);
            _view._numberValidationView._phoneNumberPreviewText.text = "It was sent to " + number;
        }
        
        
        public void EditPhoneNumber(string inputText)
        {
            var phoneNumber = inputText;
            _view._phoneNumberDisplay.text = phoneNumber;


            //first dash in phone number
            if (phoneNumber.Length > 3)
            {
                var phoneNumberVisual = _view._phoneNumberDisplay.text.Insert(3, "-");
                _view._phoneNumberDisplay.text = phoneNumberVisual;
            }

            if (phoneNumber.Length > 7)
            {
                var phoneNumberVisual = _view._phoneNumberDisplay.text.Insert(7, "-");
                _view._phoneNumberDisplay.text = phoneNumberVisual;
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
                _view._numberInputField.DeactivateInputField();
            }

            //keeps phone number length correct
            if (phoneNumber.Length > 10)
            {
                _view._numberInputField.text = _view._numberInputField.text.Substring(0, _view._numberInputField.text.Length - 1);
            }
        }

    }
}