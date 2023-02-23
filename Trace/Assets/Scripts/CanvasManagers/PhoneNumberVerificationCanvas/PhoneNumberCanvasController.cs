using Firebase.Auth;

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
            PhoneAuthProvider provider = PhoneAuthProvider.GetInstance(FirebaseAuth.DefaultInstance);
            
            provider.VerifyPhoneNumber(_view._numberInputField.text, 30, null,
                verificationCompleted: (credential) => {
                    // Auto-sms-retrieval or instant validation has succeeded (Android only).
                    // There is no need to input the verification code.
                    // `credential` can be used instead of calling GetCredential().
                },
                verificationFailed: (error) => {
                    // The verification code was not sent.
                    // `error` contains a human readable explanation of the problem.
                },
                codeSent: (id, token) => {
                    // Verification code was successfully sent via SMS.
                    // `id` contains the verification id that will need to passed in with
                    // the code from the user when calling GetCredential().
                    // `token` can be used if the user requests the code be sent again, to
                    // tie the two requests together.
                },
                codeAutoRetrievalTimeOut: (id) => {
                    // Called when the auto-sms-retrieval has timed out, based on the given
                    // timeout parameter.
                    // `id` contains the verification id of the request that timed out.
                });


        }
    }
}