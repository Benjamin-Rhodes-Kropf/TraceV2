using System;
using System.Collections;
using System.Collections.Generic;
using SA.iOS.UIKit;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EditProfileDetailsManager : MonoBehaviour
{
    //Textfields references
    [SerializeField]
    private TMP_Text diplayName;
    [SerializeField]
    private TMP_Text userName;

    //profile pic reference
    public RawImage profilePicture;

    //Editable textfield reference
    [SerializeField]
    private TMP_Text editableField;
    //bools used to do work in this multipurpose screen prefab
    [SerializeField] private bool isDisplayNameScreen, isEmailScreen, isPasswordScreen;

    //action used for callback on firebase data handling
    Action<bool> onSuccessfullyDataUpdated;
    Action<string> onSucceed;

    void OnEnable()
    {
        //assiging the latest profile picture when this screen is enabled
        if (FbManager.instance.userImageTexture != null)
            profilePicture.texture = FbManager.instance.userImageTexture;

        //check expection if userdata is not null
        if (FbManager.instance.thisUserModel != null)
        {
            //assigning all the text field values
            diplayName.text = FbManager.instance.thisUserModel.DisplayName;
            userName.text = FbManager.instance.thisUserModel.Username;
        }
    }
    void Start()
    {
        //assigning callbacks/actions required by firebase to handle data on success
        onSuccessfullyDataUpdated += DataUpdated;
        onSucceed += DatFetched;
    }
    //get the values and upate in the firebase profile
    public void ApplyAndSave()
    {
        //check if screen is for display name change
        if (isDisplayNameScreen)
        {
            StartCoroutine(FbManager.instance.SetUserNickName(editableField.text, onSuccessfullyDataUpdated));
        }
        //check if screen is for Email change
        else if (isEmailScreen){
            //FbManager.instance.UpdateUserEmail(editableField.text);
            //StartCoroutine(FbManager.instance.SetUserEmailAddress(editableField.text, onSuccessfullyDataUpdated));
        }
        //check if screen is for Password change
        else if (isPasswordScreen)
        {
            //StartCoroutine(FbManager.instance.SetUserPassword(editableField.text, onSuccessfullyDataUpdated));
        }
        Debug.Log("Data Update Requested");

    }
    void DataUpdated(bool success) {
        Debug.Log("Data Updated successfully");
        FbManager.instance.FetchLatestUserDataAndAssign();
        if (isDisplayNameScreen)
        {
            diplayName.text = editableField.text;
            ShowCorfirmation("Display Name");
        }
        else if (isEmailScreen)
        {

        }
        else if (isPasswordScreen)
        {

        }
    }
    void DatFetched(string val) {
        Debug.Log(val);
    }
    void ShowCorfirmation(string dateUpdateType) {
        ISN_UIAlertController alert = new ISN_UIAlertController("Data Update", dateUpdateType+ " updated successfully!", ISN_UIAlertControllerStyle.Alert);
        ISN_UIAlertAction defaultAction = new ISN_UIAlertAction("Ok", ISN_UIAlertActionStyle.Default, () => {
            //Do something
        });

        alert.AddAction(defaultAction);
        alert.Present();
    }
}
