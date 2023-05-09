using System;
using System.Collections;
using System.Collections.Generic;
using CanvasManagers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VoxelBusters.EssentialKit;

public class ContactView : MonoBehaviour
{
   public TMP_Text _givenName;
   public TMP_Text _phoneNumber;
   public Image _contactImage;
   public Button _addButton;
   public Button _removeButton;


   public void UpdateContactInfo(IAddressBookContact contact)
   {
      _givenName.text = contact.FirstName + " "+ contact.LastName;
      _phoneNumber.text = contact.PhoneNumbers[0];
      contact.LoadImage((result, error) =>
      {
         var texture = result.GetTexture();
         if (texture)
         {
            var sprite = Sprite.Create(
               texture,
               new Rect(0, 0, texture.width, texture.height),
               new Vector2(0.5f, 0.5f));

            _contactImage.sprite = sprite;
         }
      });
      
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
