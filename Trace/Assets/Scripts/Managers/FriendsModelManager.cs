using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FriendsModelManager 
{
    private static FriendsModelManager instance = null;

    public static FriendsModelManager Instance
    {
        get
        {
            if (instance == null)
                instance = new FriendsModelManager();

            return instance;
        }
    }
    private FriendsModelManager()
    {
        
    }

    public FriendModel GetFriendModelByOtherFriendID(string otherFriend)
    {
        var friend =
            
            (from fri in FbManager.instance._allFriends
                where fri.friend2.Equals(otherFriend)
                select fri).First();

        return friend;
    }

    public bool IsAlreadyFriend(string otherFriend)
    {
        try
        {
            GetFriendModelByOtherFriendID(otherFriend);
            return true;
        }
        catch
        {
            return false;
        }        
    }


    public void RemoveFriendFromList(string otherFriend)
    {
        var friend = GetFriendModelByOtherFriendID(otherFriend);
        FbManager.instance._allFriends.Remove(friend);
    }


    public string GetFriendsID(string otherFriend)
    {
        string friendsID = "";
        var friend = GetFriendModelByOtherFriendID(otherFriend);
        friendsID = friend.friendsID;
        return friendsID;
    }
}
