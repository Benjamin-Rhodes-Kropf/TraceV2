using Firebase.Auth;
using UnityEngine;

namespace CanvasManagers
{
    public class RegisterCanvasController
    {
        private bool _isEmailValidated = false;
        private bool _isPasswordValidated = false;
        private bool _isConfirmedPasswordValidated = false;

        private readonly RegisterCanvas _view;

        public RegisterCanvasController(RegisterCanvas view)
        {
            _view = view;
        }

        public void Init()
        {
            BindEvents();
            EnableRegistrationButtonCheck();
        }

        public void Uninitialise()
        {
            UnbindEvents();
        }
        
        #region Events

        private void BindEvents()
        {
            // _view.usernameText.onEndEdit.AddListener(ValidateUsername);
            _view.emailText.onEndEdit.AddListener(ValidateEmail);
            
            _view.passwordText.onEndEdit.AddListener(ValidatePassword);
            _view.retypePasswordText.onEndEdit.AddListener(ValidatePassword);
            
            _view.registerButton.onClick.AddListener(OnClickRegister);
        }

        private void OnClickRegister()
        {
            _view.LoadingState(true);
            string email = "";
             email = _view.emailText.text;

#if UNITY_EDITOR
#else
             // email = _view.emailText.text.Replace('.', ',');
#endif
            
            Debug.LogError("Email is ::  "+ email);
            _view.StartCoroutine(FbManager.instance.RegisterNewUser(email, _view.passwordText.text, "UserName", "", (response, errorCode) =>
            {
                 _view.LoadingState(false);
                Debug.Log("Registered Response received from Firebase: " + response);
                if (errorCode == AuthError.EmailAlreadyInUse)
                {
                    _view.ShowMessage(response);
                    
                }
                else
                {
                    ScreenManager.instance.ChangeScreenForwards("PhoneNumber");
                }
            }));
        }

        private void UnbindEvents()
        {
            // _view.usernameText.onEndEdit.RemoveAllListeners();
            _view.emailText.onEndEdit.RemoveAllListeners();
            
            _view.passwordText.onEndEdit.RemoveAllListeners();
            _view.retypePasswordText.onEndEdit.RemoveAllListeners();
            
            _view.registerButton.onClick.RemoveAllListeners();
        }

        #endregion
        
        #region Validations

        private void ValidateEmail(string email)
        {
            _isEmailValidated = HelperMethods.IsEmailValid(email);
            if (!_isEmailValidated)
                _view.ShowMessage("Email address not valid. Please enter valid email address.");
            
            EnableRegistrationButtonCheck();
        }

        private void ValidateConfirmPassword(string arg0)
        {
            
        }

        private void ValidatePassword(string password)
        {
            string passwordResponse = HelperMethods.IsValidPassword(password);
            
            if (passwordResponse == null)
            {
                if (_view.passwordText.text.Equals(_view.retypePasswordText.text))
                    _isPasswordValidated = true;
                else
                {
                    _isPasswordValidated = false;
                    _view.ShowMessage("Passowrds do not match");
                }
                    
            }
            else
            {
                _view.ShowMessage(passwordResponse);
                _isPasswordValidated = false;
            }

            EnableRegistrationButtonCheck();

        }
 

        #endregion
        
        private void EnableRegistrationButtonCheck()
        {
            if (_isEmailValidated && _isPasswordValidated)
                _view.registerButton.interactable = true;
            else
                _view.registerButton.interactable = false;
        }
    }
}