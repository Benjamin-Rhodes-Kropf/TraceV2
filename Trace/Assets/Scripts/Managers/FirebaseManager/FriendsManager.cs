using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Database;
using Unity.VisualScripting;

public partial class FbManager
{
    [HideInInspector] public List<string> _previousRequestFrom;
    public List<FriendRequests> _allRequests;
    public List<FriendModel> _allFriends;

    private void HandleFriendRequest(object sender, ChildChangedEventArgs args)
    {
        if (args.Snapshot != null && args.Snapshot.Value != null)
        {
            string senderId = args.Snapshot.Child("senderId").Value.ToString();
            if (FriendRequestManager.Instance.IsRequestAllReadyInList(senderId))
            {
                _databaseReference.Child("allFriendRequests").ChildAdded -= HandleFriendRequest;
                return;
            }

            // Get the friend request data
            string requestId = args.Snapshot.Key;
            string receiverId = args.Snapshot.Child("receiverId").Value.ToString();
            string status = args.Snapshot.Child("status").Value.ToString();

            // Display the friend request to the user and provide options to accept or decline it
            Debug.LogError("Received friend request from " + senderId);
            FriendRequests request = new FriendRequests();
            request.RequestID = requestId;
            request.ReceiverId = receiverId;
            request.SenderID = senderId;
            _allRequests.Add(request);
            // Display friend request UI here...
        }
    }


    IEnumerator CheckForFriendRequest()
    {
        while (true)
        {
            yield return new WaitForSeconds(_timeToRepeatForCheckingRequest);
            _databaseReference.Child("allFriendRequests").ChildAdded += HandleFriendRequest;
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

        while (task.IsCompleted is false)
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

    public IEnumerator AcceptFriendRequest(string requestId, string senderId, Action<bool> callback)
    {
        // Update the friend request status to accepted
        _databaseReference.Child("allFriendRequests").Child(requestId).RemoveValueAsync();
        string friendId = _databaseReference.Child("Friends").Push().Key;
        Dictionary<string, object> friendsData = new Dictionary<string, object>();
        friendsData["friend1"] = _firebaseUser.UserId;
        friendsData["friend2"] = senderId;
        
        
        var task = _databaseReference.Child("Friends").Child(friendId).SetValueAsync(friendsData);

        while (task.IsCompleted is false)
            yield return new WaitForEndOfFrame();

        if (task.IsCanceled || task.IsFaulted)
        {
            print(task.Exception.Message);
            callback(false);
        }
        else
        {
            // Add friends to each other's friend list
            // _databaseReference.Child("Friends").Child(_firebaseUser.UserId).Child(requestId).SetValueAsync(true);
            // _databaseReference.Child("Friends").Child(requestId).Child(_firebaseUser.UserId).SetValueAsync(true);
            callback(true);
        }
        
    }

    public void CancelFriendRequest(string requestId)
    {
        // Delete the friend request node
        _databaseReference.Child("allFriendRequests").Child(requestId).RemoveValueAsync();
    }

    #region Query Functions

    public IEnumerator RetrieveFriendRequests()
    {
        // Get the current user's ID
        string userId = _firebaseUser.UserId;
        DatabaseReference friendRequestsRef = _databaseReference.Child("allFriendRequests");

        var task = friendRequestsRef.OrderByChild("receiverId").EqualTo(userId).GetValueAsync();
        while (task.IsCompleted is false)
            yield return new WaitForEndOfFrame();

        if (task.IsCanceled || task.IsFaulted)
        {
            print(task.Exception.Message);
        }
        else
        {
            DataSnapshot snapshot = task.Result;
            foreach (var request in snapshot.Children)
            {
                string senderId = request.Child("senderId").Value.ToString();
                if (FriendRequestManager.Instance.IsRequestAllReadyInList(senderId) is false)
                {
                    FriendRequests req = new FriendRequests();
                    req.SenderID = senderId;
                    req.ReceiverId = request.Child("receiverId").Value.ToString();
                    req.RequestID = request.Key;
                    _allRequests.Add(req);
                }
            }
        }
    }

    public void GetSpecificUserData(string userId, Action<UserModel> callBack)
    {
        StartCoroutine(GetSpecificUserDataCoroutine(userId, callBack));
    }


    private IEnumerator GetSpecificUserDataCoroutine(string userId, Action<UserModel> callBack)
    {
        // Get a reference to the "users" node in the database
        DatabaseReference usersRef = _databaseReference.Child("users");

        // Attach a listener to the "users" node
        var task = usersRef.Child(userId).GetValueAsync();

        while (task.IsCompleted is false)
            yield return new WaitForEndOfFrame();

        if (task.IsCompleted)
        {
            DataSnapshot snapshot = task.Result;
            string email = snapshot.Child("email").Value.ToString();
            string frindCount = snapshot.Child("friendCount").Value.ToString();
            string displayName = snapshot.Child("name").Value.ToString();
            string username = snapshot.Child("username").Value.ToString();
            string phoneNumber = snapshot.Child("phone").Value.ToString();
            string photoURL = snapshot.Child("userPhotoUrl").Value.ToString();
            UserModel user = new UserModel(_firebaseUser.UserId, email, int.Parse(frindCount), displayName, username,
                phoneNumber, photoURL);
            callBack(user);
        }

        if (task.IsFaulted)
        {
            Debug.LogError(task.Exception);
        }
    }

    #endregion





    #region Friendship

    public IEnumerator RetrieveFriends()
    {
        // Get the current user's ID
        string userId = _firebaseUser.UserId;
        DatabaseReference friendRequestsRef = _databaseReference.Child("Friends");

        var task = friendRequestsRef.OrderByChild("friend1").EqualTo(userId).GetValueAsync();
        while (task.IsCompleted is false)
            yield return new WaitForEndOfFrame();

        if (task.IsCanceled || task.IsFaulted)
        {
            print(task.Exception.Message);
        }
        else
        {
            DataSnapshot snapshot = task.Result;
            foreach (var friend in snapshot.Children)
            {
                string friend2 = friend.Child("friend2").Value.ToString();
                if (FriendsModelManager.Instance.IsAlreadyFriend(friend2) is false)
                {
                    FriendModel fri = new FriendModel();
                    fri.friend1 = _firebaseUser.UserId;
                    fri.friend2 = friend.Child("friend2").Value.ToString();
                    fri.friendsID = friend.Key;
                    _allFriends.Add(fri);
                }
            }
        }
    }

    #endregion
}