using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Database;

public partial class FbManager
{

    private void HandleFriendRequest(object sender, ChildChangedEventArgs args)
    {
        if (args.Snapshot != null && args.Snapshot.Value != null)
        {
            // Get the friend request data
            string requestId = args.Snapshot.Key;
            string senderId = args.Snapshot.Child("senderId").Value.ToString();
            string receiverId = args.Snapshot.Child("receiverId").Value.ToString();
            string status = args.Snapshot.Child("status").Value.ToString();

            // Display the friend request to the user and provide options to accept or decline it
            Debug.Log("Received friend request from " + senderId);
            // Display friend request UI here...
        }
    }
    
    
    

    public IEnumerator SendFriendRequest(string friendId, Action<bool> callback)
    {
        // Create a new friend request node
        string requestId = _databaseReference.Child("allFriendRequests").Push().Key;
        Dictionary<string, object> requestData = new Dictionary<string, object>();
        requestData["senderId"] = _firebaseUser.UserId;
        requestData["receiverId"] = friendId;
        requestData["status"] = "pending";
        var task = _databaseReference.Child("allFriendRequests").Child(requestId).SetValueAsync(requestData);

        while (task.IsCompleted is  false)
            yield return new WaitForEndOfFrame();

        if (task.IsCanceled || task.IsFaulted)
        {
            print(task.Exception.Message);
            callback(false);
        }
        else
        {
            callback(true);
        }
        
        
    }

    public void AcceptFriendRequest(string requestId)
    {
        // Update the friend request status to accepted
        _databaseReference.Child("allFriendRequests").Child(requestId).Child("status").SetValueAsync("accepted");

        // Add friends to each other's friend list
        _databaseReference.Child("Friends").Child(_firebaseUser.UserId).Child(requestId).SetValueAsync(true);
        _databaseReference.Child("Friends").Child(requestId).Child(_firebaseUser.UserId).SetValueAsync(true);
    }

    public void CancelFriendRequest(string requestId)
    {
        // Delete the friend request node
        _databaseReference.Child("allFriendRequests").Child(requestId).RemoveValueAsync();
    }
}