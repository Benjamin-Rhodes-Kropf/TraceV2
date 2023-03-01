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
        }

        public void Uninitilise()
        {
            _view.verifyNumberButton.onClick.RemoveAllListeners();
        }

        private void OnVerifyNumberClicked()
        {
            Debug.LogError("In Varify Phone !!");
            PhoneAuthProvider provider = PhoneAuthProvider.GetInstance(FirebaseAuth.DefaultInstance);
            
            provider.VerifyPhoneNumber("+92"+_view._numberInputField.text.Trim(), 30, null,
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


        }
    }
}