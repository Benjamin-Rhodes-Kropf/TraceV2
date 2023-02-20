using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CanvasManagers
{
    public class RegisterCanvas : MonoBehaviour
    {
        [Header("Canvas Components")]
        [SerializeField] private TMP_InputField usernameText;
        [SerializeField] private TMP_InputField emailText;
        [SerializeField] private TMP_InputField passwordText;
        [SerializeField] private TMP_InputField retypePasswordText;
        [SerializeField] private Button registerButton;

        private bool _isEmailValidated = false;
        private bool _isUsernameValidated = false;
        private bool _isPasswordValidated = false;
        
    
        private void OnEnable()
        {
            Debug.Log("Register Canvas Enabled");

            BindEvents();
            registerButton.interactable = false;
        }

        private void OnDisable()
        {
            UnbindEvents();
        }


        #region Events

        private void BindEvents()
        {
            usernameText.onEndEdit.AddListener(ValidateUsername);
            emailText.onEndEdit.AddListener(ValidateEmail);
            
            passwordText.onEndEdit.AddListener(ValidatePassword);
            retypePasswordText.onEndEdit.AddListener(ValidateConfirmPassword);
        }
        
        private void UnbindEvents()
        {
            usernameText.onEndEdit.RemoveAllListeners();
            emailText.onEndEdit.RemoveAllListeners();
            
            passwordText.onEndEdit.RemoveAllListeners();
            retypePasswordText.onEndEdit.RemoveAllListeners();
        }

        #endregion
        
        private void ValidateEmail(string email)
        {
           _isEmailValidated = HelperMethods.IsEmailValid(email);
        }

        private void ValidateConfirmPassword(string arg0)
        {
            
        }

        private void ValidatePassword(string password)
        {
            if (HelperMethods.IsValidPassword(password) == null)
            {
                
            }
        }

        private void ValidateUsername(string username)
        {
            _isUsernameValidated = !HelperMethods.isBadName(username);
        }

        private void Register(string username, string email, string password, string confirmPassword)
        {
            StartCoroutine(FbManager.instance.RegisterNewUser(email, password, username, "", (response) =>
            {
                Debug.Log("Registered Response received from Firebase: " + response);
            }));
        }
        
        
    }
}