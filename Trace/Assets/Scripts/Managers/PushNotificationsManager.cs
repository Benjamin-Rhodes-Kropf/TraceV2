using System;
using System.Collections;
using Firebase;
using Helper;
// using Unity.Notifications.iOS;
using UnityEngine;

public class PushNotificationsManager : UnitySingleton<PushNotificationsManager>
{
    private void OnEnable()
    {
        Init();
    }

    private void Init()
    {

        // Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        // {
        //     var dependencyStatus = task.Result;
        //     if (dependencyStatus == DependencyStatus.Available)
        //     {
        //         Firebase.FirebaseApp app = FirebaseApp.DefaultInstance;
        //     }
        //     else
        //     {
        //         Debug.LogError("Could Not Resolve all firebase dependencies : " + dependencyStatus);
        //     }
        // });
        
        
        
        // Firebase.Messaging.FirebaseMessaging.TokenRegistrationOnInitEnabled = true;
        //
        //
        // Firebase.Messaging.FirebaseMessaging.TokenReceived += OnTokenReceived;
        // Firebase.Messaging.FirebaseMessaging.MessageReceived += OnMessageReceived;
        // Firebase.Messaging.FirebaseMessaging.SubscribeAsync("all");
    }


    // private void OnTokenReceived(object sender, Firebase.Messaging.TokenReceivedEventArgs token) {
    //     UnityEngine.Debug.Log("Received Registration Token: " + token.Token);
    //     // Constants.DEVICE_NOTIFICATION_TOKEN = token.Token;
    // }
    //
    // private void OnMessageReceived(object sender, Firebase.Messaging.MessageReceivedEventArgs e) {
    //     UnityEngine.Debug.Log("Received a new message from: " + e.Message.From);
    // }

}