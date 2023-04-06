using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Unity.VisualScripting;

public class FriendRequestManager
{
    private static FriendRequestManager instance = null;
    public List<FriendRequests> sentRequests;
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
        sentRequests = new List<FriendRequests>();
    }

    public FriendRequests GetRequestBySenderID(string senderId, bool isReceivedRequest = true)
    {
        if (isReceivedRequest)
        {
                FriendRequests friendRequest =

                    (from request in FbManager.instance._allReceivedRequests
                        where request.SenderID.Equals(senderId)
                        select request).First();

                return friendRequest;
        }
        else
        {
                FriendRequests friendRequest =
                    (from request in FbManager.instance._allSentRequests
                        where request.ReceiverId.Equals(senderId)
                        select request).First();
                return friendRequest;
        }
    }
    

    public bool IsRequestAllReadyInList(string senderId, bool isReceivedRequest = true)
    {
        try
        {
            GetRequestBySenderID(senderId, isReceivedRequest);
            return true;
        }
        catch
        {
            return false;
        }        
    }


    public void RemoveRequestFromList(string senderId)
    {
        var request = GetRequestBySenderID(senderId);
        FbManager.instance._allReceivedRequests.Remove(request);
    }


    public string GetRequestID(string senderId, bool isReceivedRequest = true)
    {
        string requestID = "";
        var request = GetRequestBySenderID(senderId,  isReceivedRequest);
        requestID = request.RequestID;
        return requestID;
    }
    
}
