using System;
using System.Collections;
using System.Collections.Generic;
using CanvasManagers;
using Firebase.Database;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR;

public partial class FbManager
{
    public Dictionary<string, eFriendRequestType> _allFriendRequests;
    /// <summary>
    /// On Send Friend Request Function - Update Realtime database table and Local User Interface
    /// </summary>
    /// <param name="friendId"></param>
    /// <param name="callback"></param>
    /// <returns></returns>
    public IEnumerator SendFriendRequestAction(string friendId, Action<bool> callback)
    {
        // Add Data to Tables 
        var sentTask = _databaseReference.Child("FriendRequests").Child(_firebaseUser.UserId).Child("Sent").Child(friendId).SetValueAsync("s");
        yield return new WaitUntil(predicate: () => sentTask.IsCompleted);
        
        var receivedTask = _databaseReference.Child("FriendRequests").Child(friendId).Child("Received").Child(_firebaseUser.UserId).SetValueAsync("r");
        yield return new WaitUntil(predicate: () => receivedTask.IsCompleted);
        
        if (sentTask.IsCanceled || sentTask.IsFaulted || receivedTask.IsCanceled || receivedTask.IsFaulted)
        {
            print("Sent Task Exception "+ sentTask.Exception.Message);
            print("Received Task Exception "+ receivedTask.Exception.Message);
            callback(false);
        }
        else
        { 
            if (_allFriendRequests.ContainsKey(friendId) is false)
                _allFriendRequests.Add(friendId,eFriendRequestType.Sent);

            callback(true);
        }
    }
    /// <summary>
    /// On The User Login Get all the Requests from Database
    /// </summary>
    private void GetAllFriendRequestsFromDatabase()
    {
        // Get the current user's ID
        var userId = _firebaseUser.UserId;
        var friendRequestsRef = _databaseReference.Child("FriendRequests").Child(userId);

        friendRequestsRef.Child("Received").GetValueAsync().ContinueWith(receivedTask =>
        {
            if (receivedTask.IsCompleted)
            {
                DataSnapshot receivedSnapshot = receivedTask.Result;
                var allReceivedRequests = receivedSnapshot.Children.ToArrayPooled();
                foreach (var user in allReceivedRequests)
                    _allFriendRequests.Add(user.Key, eFriendRequestType.Received);
            }

            if (receivedTask.IsFaulted)
                MyDebug.Instance.LogError("Received Task is Faulted with Exception :: "+ receivedTask.Exception.Message);
        });
        
        friendRequestsRef.Child("Sent").GetValueAsync().ContinueWith(sentTask =>
        {
            if (sentTask.IsCompleted)
            {
                DataSnapshot sentSnapshot = sentTask.Result;
                var allSentRequests = sentSnapshot.Children.ToArrayPooled();
                foreach (var user in allSentRequests)
                    _allFriendRequests.Add(user.Key, eFriendRequestType.Sent);
            }

            if (sentTask.IsFaulted)
                MyDebug.Instance.LogError("Sent Task is Faulted with Exception :: " + sentTask.Exception.Message);
            else
                MyDebug.Instance.Log("Total Friend Requests Found :: "+ _allFriendRequests.Count);
        });
    }
    
    #region Request Listners

    private void AllRequestListners()
    {
        StartCoroutine(OnReceievedFriendRequestAdd());
        StartCoroutine(OnReceievedFriendRequestRemoved());
        StartCoroutine(OnSentFriendRequestAdd());
        StartCoroutine(OnSentFriendRequestRemoved());
    }
    
    
    private IEnumerator OnReceievedFriendRequestAdd()
    {
        while (true)
        {
            yield return new WaitForSeconds(_timeToRepeatForCheckingRequest);
            _databaseReference.Child("FriendRequests").Child(_firebaseUser.UserId).Child("Received").ChildAdded += HandleOnReceivedFriendRequestAdd;
        }
    }
    private void HandleOnReceivedFriendRequestAdd(object sender, ChildChangedEventArgs args)
    {
        try
        {
            if (args.Snapshot is not { Value: { } }) return;
            
            var requestId = args.Snapshot.Key;

            if (_allFriendRequests.ContainsKey(requestId) is false)
            {
                _allFriendRequests.Add(requestId, eFriendRequestType.Received);
                UpdateRequestUI();
            }
            
            _databaseReference.Child("FriendRequests").Child(_firebaseUser.UserId).Child("Received").ChildAdded -= HandleOnReceivedFriendRequestAdd;
        }
        catch (Exception e)
        {
            MyDebug.Instance.Log(nameof(HandleOnReceivedFriendRequestAdd) + ":: "+ e.Message);
        }
    }
    private IEnumerator OnReceievedFriendRequestRemoved()
    {
        while (true)
        {
            yield return new WaitForSeconds(_timeToRepeatForCheckingRequest);
            _databaseReference.Child("FriendRequests").Child(_firebaseUser.UserId).Child("Received").ChildRemoved += HandleOnReceivedFriendRequestRemoved;
        }
    }
    private void HandleOnReceivedFriendRequestRemoved(object sender, ChildChangedEventArgs args)
    {
        try
        {
            if (args.Snapshot is not { Value: { } }) return;
            
            var requestId = args.Snapshot.Key;

            if (_allFriendRequests.ContainsKey(requestId))
                _allFriendRequests.Remove(requestId);
          
            _databaseReference.Child("FriendRequests").Child(_firebaseUser.UserId).Child("Received").ChildRemoved -= HandleOnReceivedFriendRequestRemoved;
        }
        catch (Exception e)
        {
            MyDebug.Instance.Log(nameof(HandleOnReceivedFriendRequestRemoved) + ":: "+ e.Message);
        }
    }

    // Request Sent Listners
    private IEnumerator OnSentFriendRequestAdd()
    {
        while (true)
        {
            yield return new WaitForSeconds(_timeToRepeatForCheckingRequest);
            _databaseReference.Child("FriendRequests").Child(_firebaseUser.UserId).Child("Sent").ChildAdded += HandleOnSentFriendRequestAdd;
        }
    }
    private void HandleOnSentFriendRequestAdd(object sender, ChildChangedEventArgs args)
    {
        try
        {
            if (args.Snapshot is not { Value: { } }) return;
            
            var requestId = args.Snapshot.Key;
            
            if (_allFriendRequests.ContainsKey(requestId) is false)
                _allFriendRequests.Add(requestId, eFriendRequestType.Sent);
          
            _databaseReference.Child("FriendRequests").Child(_firebaseUser.UserId).Child("Sent").ChildAdded -= HandleOnSentFriendRequestAdd;
        }
        catch (Exception e)
        {
            MyDebug.Instance.Log(nameof(HandleOnSentFriendRequestAdd) + ":: "+ e.Message);
        }
    }
    private IEnumerator OnSentFriendRequestRemoved()
    {
        while (true)
        {
            yield return new WaitForSeconds(_timeToRepeatForCheckingRequest);
            _databaseReference.Child("FriendRequests").Child(_firebaseUser.UserId).Child("Sent").ChildRemoved += HandleOnSentFriendRequestRemoved;
        }
    }
    private void HandleOnSentFriendRequestRemoved(object sender, ChildChangedEventArgs args)
    {
        try
        {
            if (args.Snapshot is not { Value: { } }) return;
            
            var requestId = args.Snapshot.Key;
            
            if (_allFriendRequests.ContainsKey(requestId))
                _allFriendRequests.Remove(requestId);
          
            _databaseReference.Child("FriendRequests").Child(_firebaseUser.UserId).Child("Sent").ChildRemoved -= HandleOnSentFriendRequestRemoved;
        }
        catch (Exception e)
        {
            MyDebug.Instance.Log(nameof(HandleOnSentFriendRequestRemoved) + ":: "+ e.Message);
        }
    }
    private void UpdateRequestUI()
    {
        SoundManager.instance.PlaySound(SoundManager.SoundType.Notification);
        HelperMethods.PlayHeptics();
        ContactsCanvas.UpdateRedMarks?.Invoke();

        if (ContactsCanvas.UpdateRequestView != null)
            ContactsCanvas.UpdateRequestView?.Invoke();
    }
    #endregion
    
}

public enum eFriendRequestType
{
    Sent, Received
}