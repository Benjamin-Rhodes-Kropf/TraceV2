using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using CanvasManagers;

public class FriendRequestManager
{
    private static FriendRequestManager instance = null;
    public Dictionary<string, FriendRequests> _allSentRequests;


    public static FriendRequestManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new FriendRequestManager();
                
            }

            return instance;
        }
    }
    private FriendRequestManager()
    { 
        _allSentRequests = new Dictionary<string, FriendRequests>();
    }


    private FriendRequests GetRequestBySenderID(string senderId, bool isReceivedRequest = true)
    {
        RemoveDuplicates();
        if (isReceivedRequest)
        {
                FriendRequests friendRequest =

                    (from request in FbManager.instance._allReceivedRequests
                        where request.SenderID.Equals(senderId)
                        select request).FirstOrDefault();

                return friendRequest;
        }
        else
        {
                foreach (var id in _allSentRequests.Keys)
                {
                    var isExist = _allSentRequests[id].ReceiverId == senderId;
                    if (isExist)
                        return _allSentRequests[id];
                }
                return null;
        }
    }

    private FriendRequests GetRequestByRequestID(string requestId, bool isRecievedRequest = true)
    {
        if (isRecievedRequest)
        {
            FriendRequests friendRequest =
                (from request in FbManager.instance._allReceivedRequests
                    where request.RequestID.Equals(requestId)
                    select request).FirstOrDefault();
            return friendRequest;
        }
        else
        {
            if (_allSentRequests.ContainsKey(requestId))
                return _allSentRequests[requestId];
            else
                return null;
        }
    }

    public bool IsRequestAllReadyInList(string senderId, bool isReceivedRequest = true)
    {
        try
        {
            var request = GetRequestBySenderID(senderId, isReceivedRequest);
            if (request == null)
                return false;

            return true;
        }
        catch(Exception e)
        {
            Debug.Log("Exception Caught in FriendsRequestManager.IsRequestAllReadyInList "+ e.Message);
            return false;
        }        
    }


    public void RemoveRequestFromList(string senderId, bool isReceivedRequest  = true)
    {
        var request = GetRequestBySenderID(senderId, isReceivedRequest);

        if (isReceivedRequest)
            FbManager.instance._allReceivedRequests.Remove(request);
        else
            _allSentRequests.Remove(senderId);
    }


    public string GetRequestID(string senderId, bool isReceivedRequest = true)
    {
        string requestID = "";
        var request = GetRequestBySenderID(senderId,  isReceivedRequest);
        
        if (request != null)
            requestID = request.RequestID;
        
        return requestID;
    }

    public void RemoveRequest(string requestId)
    {
        var receivedRequest = GetRequestByRequestID(requestId);
        if (receivedRequest != null)
            FbManager.instance._allReceivedRequests.Remove(receivedRequest);
        else
        {
            _allSentRequests.Remove(requestId);
        }
        if (ContactsCanvas.UpdateRequestView != null)
            ContactsCanvas.UpdateRequestView?.Invoke();

    }

    public void RemoveRequestByUserID(string userId)
    {
        var receivedRequest = GetRequestBySenderID(userId);
        if (receivedRequest != null)
            FbManager.instance._allReceivedRequests.Remove(receivedRequest);
        else
        {
            _allSentRequests.Remove(receivedRequest.RequestID);
        }
        if (ContactsCanvas.UpdateRequestView != null)
            ContactsCanvas.UpdateRequestView?.Invoke();
    }
    

    private void RemoveDuplicates()
    {
        List<FriendRequests> distinctList1 = FbManager.instance._allReceivedRequests.Distinct().ToList();
        if (distinctList1.Count() != FbManager.instance._allReceivedRequests.Count())
        {
            FriendRequests duplicateItem = FbManager.instance._allReceivedRequests.Except(distinctList1).FirstOrDefault();
            FbManager.instance._allReceivedRequests.RemoveAll(item => item.Equals(duplicateItem));
        }
    }
    
}
