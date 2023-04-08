using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class FriendModel : IEquatable<FriendModel>
{
    public string friend;
    public bool Equals(FriendModel other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return friend == other.friend;
    }

    public override int GetHashCode()
    {
        return (friend != null ? friend.GetHashCode() : 0);
    }
}
