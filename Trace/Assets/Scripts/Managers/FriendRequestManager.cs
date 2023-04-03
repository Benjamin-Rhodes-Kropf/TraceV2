using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

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

    public FriendRequests GetRequestBySenderID(string senderId)
    {
        FriendRequests friendRequest =
            
            (from request in FbManager.instance._allRequests
            where request.SenderID.Equals(senderId)
            select request).First();

        return friendRequest;
    }

    public bool IsRequestAllReadyInList(string senderId)
    {
        try
        {
            GetRequestBySenderID(senderId);
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
        FbManager.instance._allRequests.Remove(request);
    }


    public string GetRequestID(string senderId)
    {
        string requestID = "";
        var request = GetRequestBySenderID(senderId);
        requestID = request.RequestID;
        return requestID;
    }
    
}
