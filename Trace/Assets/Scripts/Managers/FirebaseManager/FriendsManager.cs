using System;
using System.Collections;
using System.Collections.Generic;
using CanvasManagers;
using UnityEngine;
using Firebase.Database;


public partial class FbManager
{
    [HideInInspector] public List<string> _previousRequestFrom;
    public List<FriendRequests> _allReceivedRequests;
    public List<FriendRequests> _allSentRequests;
    public List<FriendModel> _allFriends;

    #region Continues Listners
    private void HandleFriendRequest(object sender, ChildChangedEventArgs args)
    {
        try
        {
            if (args.Snapshot != null && args.Snapshot.Value != null)
            {
                string senderId = args.Snapshot.Child("senderId").Value.ToString();
                string receiverId = args.Snapshot.Child("receiverId").Value.ToString();

                if (receiverId != _firebaseUser.UserId)
                {
                    _databaseReference.Child("allFriendRequests").ChildAdded -= HandleFriendRequest;
                    return;
                }
                
                if (FriendRequestManager.Instance.IsRequestAllReadyInList(senderId) || senderId == _firebaseUser.UserId)
                {
                    _databaseReference.Child("allFriendRequests").ChildAdded -= HandleFriendRequest;
                    return;
                }

                print("Receiver ID : "+ receiverId);
                print("Sender ID : "+ senderId);
                
                // Get the friend request data
                string requestId = args.Snapshot.Key;
                string status = args.Snapshot.Child("status").Value.ToString();

                // Display the friend request to the user and provide options to accept or decline it
                var request = new FriendRequests
                {
                    RequestID = requestId,
                    ReceiverId = receiverId,
                    SenderID = senderId
                };
                
                _allReceivedRequests.Add(request);
                SoundManager.instance.PlaySound(SoundManager.SoundType.Notification);
                HelperMethods.PlayHeptics();
                ContactsCanvas.UpdateRedMarks?.Invoke();

                if (ContactsCanvas.UpdateRequestView != null)
                    ContactsCanvas.UpdateRequestView?.Invoke();
                
                // Display friend request UI here...
            }
        
        }
        catch (Exception e)
        {
            _databaseReference.Child("allFriendRequests").ChildAdded -= HandleFriendRequest;
            // Debug.Log("Exception From HandleFriendRequest");
        }
    }
    private void HandleRemovedRequests(object sender, ChildChangedEventArgs args)
    {
        try
        {
            if (args.Snapshot is not { Value: { } }) return;
            
            string requestId = args.Snapshot.Key;
            
            FriendRequestManager.Instance.RemoveRequest(requestId);
          
            _databaseReference.Child("allFriendRequests").ChildRemoved -= HandleRemovedRequests;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }
    private void HandleFriends(object sender, ChildChangedEventArgs args)
    {
        try
        {
            if (args.Snapshot == null || args.Snapshot.Value == null) return;
            var friendId = args.Snapshot.Key.ToString();

            if (string.IsNullOrEmpty(friendId)) return;
            
            if (FriendsModelManager.Instance.IsAlreadyFriend(friendId) || friendId == _firebaseUser.UserId)
            {
                _databaseReference.Child("Friends").Child(_firebaseUser.UserId).ChildAdded -= HandleFriends;
                return;
            }
            // Display the friend request to the user and provide options to accept or decline it
            var friend = new FriendModel
            {
                friend = friendId
            };
            
            print("New Friend Added :: "+ friendId);
            _allFriends.Add(friend);
            if (ContactsCanvas.UpdateFriendsView != null)
                ContactsCanvas.UpdateFriendsView?.Invoke();
            
            _databaseReference.Child("Friends").Child(_firebaseUser.UserId).ChildAdded -= HandleFriends;

        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }
    private void HandleRemovedFriends(object sender, ChildChangedEventArgs args)
    {
        try
        {
            if (args.Snapshot == null || args.Snapshot.Value == null) return;
            var friendId = args.Snapshot.Key.ToString();
            FriendsModelManager.Instance.RemoveFriendFromList(friendId);
            
           
            
            _databaseReference.Child("Friends").Child(_firebaseUser.UserId).ChildRemoved -= HandleRemovedFriends;

        }
        catch (Exception e)
        {
            Console.WriteLine(e);
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

    IEnumerator CheckIfFriendRequestRemoved()
    {
        while (true)
        {
            yield return new WaitForSeconds(_timeToRepeatForCheckingRequest);
            _databaseReference.Child("allFriendRequests").ChildRemoved += HandleRemovedRequests;
        }
    }


    IEnumerator CheckForNewFriends()
    {
        while (true)
        {
            yield return new WaitForSeconds(_timeToRepeatForCheckingRequest);
            _databaseReference.Child("Friends").Child(_firebaseUser.UserId).ChildAdded += HandleFriends;
        }
    }
    
    IEnumerator CheckIfFriendRemoved()
    {
        while (true)
        {
            yield return new WaitForSeconds(_timeToRepeatForCheckingRequest);
            _databaseReference.Child("Friends").Child(_firebaseUser.UserId).ChildRemoved += HandleRemovedFriends;
        }
    }

    #endregion

    public IEnumerator SendFriendRequest(string friendId, Action<bool> callback)
    {
        string requestId = _databaseReference.Child("allFriendRequests").Push().Key;
        Dictionary<string, object> requestData = new Dictionary<string, object>();
        requestData["senderId"] = _firebaseUser.UserId;
        requestData["receiverId"] = friendId;
        requestData["status"] = "pending";

        var request = new FriendRequests()
        {
            ReceiverId = friendId,
            RequestID = requestId,
            SenderID = _firebaseUser.UserId
        };
        
        // Create a new friend request node
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
            FriendRequestManager.Instance._allSentRequests.Add(requestId,request);
            callback(true);
        }
    }
    public IEnumerator AcceptFriendRequest(string requestId, string senderId, Action<bool> callback)
    {
        _databaseReference.Child("allFriendRequests").Child(requestId).RemoveValueAsync();
        
        // var friend = new FriendModel
        // {
        //     friend = senderId
        // };

        print("Friend Request Accept Called With  SenderID ::  "+ senderId);
        
        var task = _databaseReference.Child("Friends").Child(_firebaseUser.UserId).Child(senderId).SetValueAsync(false);
        _databaseReference.Child("Friends").Child(senderId).Child(_firebaseUser.UserId).SetValueAsync(false);
        while (task.IsCompleted is false)
            yield return new WaitForEndOfFrame();

        if (task.IsCanceled || task.IsFaulted)
        {
            print(task.Exception.Message);
            callback(false);
        }
        else
        {
           
            // _allFriends.Add(friend);
            callback(true);
        }
        
    }
    
    
    public IEnumerator SetBestFriend( string friendID, bool isBestFriend, Action<bool> callback)
    {
        var task = _databaseReference.Child("Friends").Child(_firebaseUser.UserId).Child(friendID).SetValueAsync(isBestFriend);

        while (task.IsCompleted is false)
            yield return new WaitForEndOfFrame();

        if (task.IsCanceled || task.IsFaulted)
        {
            print("False Called");
            callback(false);
        }
        else
        {
            print("True Called");
            callback(true);
        }
    }
    
    public void CancelFriendRequest(string requestId)
    {
        // Delete the friend request node
        _databaseReference.Child("allFriendRequests").Child(requestId).RemoveValueAsync();
    }

    #region Query Functions

    private IEnumerator RetrieveFriendRequests()
    {
        // Get the current user's ID
        var userId = _firebaseUser.UserId;
        var friendRequestsRef = _databaseReference.Child("allFriendRequests");

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
                var senderId = request.Child("senderId").Value.ToString();
                if (FriendRequestManager.Instance.IsRequestAllReadyInList(senderId, false) is false)
                {
                    FriendRequests req = new FriendRequests();
                    req.SenderID = senderId;
                    req.ReceiverId = request.Child("receiverId").Value.ToString();
                    req.RequestID = request.Key;
                    print("Request ID :: "+ req.RequestID);
                    print("Sender ID :: "+ req.SenderID);
                    print("Receiver ID :: "+ req.ReceiverId);
                    _allReceivedRequests.Add(req);
                }
            }
        }
    }

    private IEnumerator RetrieveSentFriendRequests()
    {
        // Get the current user's ID
        string userId = _firebaseUser.UserId;
        DatabaseReference friendRequestsRef = _databaseReference.Child("allFriendRequests");

        var task = friendRequestsRef.OrderByChild("senderId").EqualTo(userId).GetValueAsync();
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
                string receiverId = request.Child("receiverId").Value.ToString();
                if (FriendRequestManager.Instance.IsRequestAllReadyInList(receiverId) is false)
                {
                    FriendRequests req = new FriendRequests();
                    req.SenderID = request.Child("senderId").Value.ToString();;
                    req.ReceiverId = receiverId;
                    req.RequestID = request.Key;
                    FriendRequestManager.Instance._allSentRequests.Add(req.RequestID,req);
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

        var task = friendRequestsRef.Child(userId).GetValueAsync();

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
                var friendID = friend.Key.ToString();
                // print("IsBest Friends :: " + friend.Value.ToString());
                var isBestFriend = false;
    
                if (friend.Value.ToString().Equals("*") is false)
                    isBestFriend= System.Convert.ToBoolean(friend.Value.ToString());
                
                if (FriendsModelManager.Instance.IsAlreadyFriend(friendID) is false)
                {
                    var fri = new FriendModel
                    {
                        friend = friendID,
                        isBestFriend = isBestFriend
                    };
                    _allFriends.Add(fri);
                }
            }
        }

        
    }


    public void RemoveFriends(string friendshipId)
    {
        // Delete the friend request node
        _databaseReference.Child("Friends").Child(_firebaseUser.UserId).Child(friendshipId).RemoveValueAsync();
        _databaseReference.Child("Friends").Child(friendshipId).Child(_firebaseUser.UserId).RemoveValueAsync();
    }

    #endregion
}