using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CanvasManagers;
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

    private FriendModel GetFriendModelByOtherFriendID(string otherFriend)
    {
        RemoveDuplicates();
        var friend =
            
            (from fri in FbManager.instance._allFriends
                where fri.friend.Equals(otherFriend)
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

    public bool IsBestFriend(string id)
    {
       return GetFriendModelByOtherFriendID(id).isBestFriend;
    }

    public void SetBestFriend(string id, bool isBestFriend)
    {
        //Todo : Update Data in Local List
        for (var i = 0; i < FbManager.instance._allFriends.Count; i++)
        {
            if (FbManager.instance._allFriends[i].friend.Equals(id))
            {
                FbManager.instance._allFriends[i].isBestFriend = isBestFriend;
                break;
            }
        }
        
    }

    public void RemoveFriendFromList(string otherFriend)
    {
        var friend = GetFriendModelByOtherFriendID(otherFriend);
        FbManager.instance._allFriends.Remove(friend);
        if (ContactsCanvas.UpdateFriendsView != null)
            ContactsCanvas.UpdateFriendsView?.Invoke();
    }


    private void RemoveDuplicates()
    {
        List<FriendModel> distinctList1 = FbManager.instance._allFriends.Distinct().ToList();
        if (distinctList1.Count() != FbManager.instance._allFriends.Count())
        {
            FriendModel duplicateItem = FbManager.instance._allFriends.Except(distinctList1).FirstOrDefault();
            FbManager.instance._allFriends.RemoveAll(item => item.Equals(duplicateItem));
        }
    }
}
