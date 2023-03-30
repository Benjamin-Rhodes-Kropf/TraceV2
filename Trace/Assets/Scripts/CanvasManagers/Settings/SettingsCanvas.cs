using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingsCanvas : MonoBehaviour
{
   public TMP_Text _usernameText;
   public TMP_Text _profileNameText;
   public Image _profileImage;
   
   private SettingCanvasController _controller;


   #region UnityEvents

   private void OnEnable()
   {
      if (_controller == null)
         _controller = new SettingCanvasController();
            
      _controller.Init(this);
   }

   private void OnDisable()
   {
      _controller.UnInitialize();
   }

   #endregion
}
