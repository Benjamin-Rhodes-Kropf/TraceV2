using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FriendRequests : IEquatable<FriendRequests>
{
    public string RequestID;
    public string SenderID;
    public string ReceiverId;

    public bool Equals(FriendRequests other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return RequestID == other.RequestID;
    }
    public override int GetHashCode()
    {
        return HashCode.Combine(RequestID, SenderID, ReceiverId);
    }
}
