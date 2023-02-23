using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CanvasManagers
{
    public class RegisterCanvas : MonoBehaviour
    {
        [Header("Canvas Components")]
        public TMP_InputField usernameText;
        public TMP_InputField emailText;
        public TMP_InputField passwordText;
        public TMP_InputField retypePasswordText;
        public Button registerButton;
        public TMP_Text errorText;


        private RegisterCanvasController _controller;
        
        #region UnityCallbacks

        private void OnEnable()
        {
            Debug.Log("Register Canvas Enabled");

            if (_controller == null)
                _controller = new RegisterCanvasController(this);
            
            _controller.Init();
            registerButton.interactable = false;
        }

        private void OnDisable()
        {
            _controller.Uninitialise();
        }

        #endregion


        public void ShowMessage(string message)
        {
            errorText.text = message;
            errorText.gameObject.SetActive(true);
            
            StartCoroutine(HelperMethods.TimedActionFunction(3f, ()=>
            {
                errorText.text = "";
                errorText.gameObject.SetActive(false);
            }));
        }
        
    }
}