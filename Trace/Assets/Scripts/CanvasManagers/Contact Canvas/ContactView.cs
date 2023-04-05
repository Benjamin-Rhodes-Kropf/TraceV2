using System;
using System.Collections;
using System.Collections.Generic;
using CanvasManagers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ContactView : MonoBehaviour
{
   public TMP_Text _givenName;
   public TMP_Text _phoneNumber;
   public Button _addButton;
   public Button _removeButton;


   public void UpdateContactInfo(Contact contact)
   {
      _givenName.text = contact.givenName;
      _phoneNumber.text = contact.phoneNumber;
      _addButton.onClick.RemoveAllListeners();
      _addButton.onClick.AddListener(OnAddClick);
      _removeButton.onClick.RemoveAllListeners();
      _removeButton.onClick.AddListener(OnRemoveClick);
   }


   private void OnAddClick()
   {
      print("On Add Clicked");
   }

   private void OnRemoveClick()
   {
      print("On Remove Clicked");
      Destroy(gameObject);
   }
}
