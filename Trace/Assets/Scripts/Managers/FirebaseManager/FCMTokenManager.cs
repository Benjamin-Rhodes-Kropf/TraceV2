using System;
using System.Collections;
using System.Collections.Generic;
using Firebase;
using UnityEngine;

public partial class FbManager
{
    private bool IsApplicationFirstTimeOpened {
        get
        {
            return System.Convert.ToBoolean(PlayerPrefs.GetInt("IsApplicationFirstTimeOpened", 1));
        }
        set
        {
            PlayerPrefs.SetInt("IsApplicationFirstTimeOpened", Convert.ToInt32(value));
            PlayerPrefs.Save();
        }
    }
   
    
    private void InitializeFCMService()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        {
            var dependencyStatus = task.Result;
            if (dependencyStatus == DependencyStatus.Available)
            {
                Firebase.Messaging.FirebaseMessaging.TokenRegistrationOnInitEnabled = true;
                Firebase.Messaging.FirebaseMessaging.TokenReceived += OnTokenReceived;
                Firebase.Messaging.FirebaseMessaging.MessageReceived += OnMessageReceived;
                Firebase.Messaging.FirebaseMessaging.SubscribeAsync("all");
            }
            else
            {
                Debug.LogError($"Firebase dependencies not available: {dependencyStatus}");
            }
        });
    }

    private void OnTokenReceived(object sender, Firebase.Messaging.TokenReceivedEventArgs token) {
        UnityEngine.Debug.Log("Received Registration Token: " + token.Token);
        
        if (!IsApplicationFirstTimeOpened) 
            return;

        StartCoroutine(SetFCMDeviceToken(token.Token));
        IsApplicationFirstTimeOpened = false;
    }
    
    private void OnMessageReceived(object sender, Firebase.Messaging.MessageReceivedEventArgs e) {
        UnityEngine.Debug.Log("Received a new message from: " + e.Message.From);
    }

    IEnumerator SetFCMDeviceToken(string token)
    {
        var DBTaskSetUserFriends = _databaseReference.Child("FcmTokens").Child(_firebaseUser.UserId).SetValueAsync(token);
        while (DBTaskSetUserFriends.IsCompleted is false)
            yield return new WaitForEndOfFrame();
    }

}
