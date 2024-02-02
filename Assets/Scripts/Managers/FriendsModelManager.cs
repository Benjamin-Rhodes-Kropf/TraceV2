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

    public FriendModel GetFriendModelByOtherFriendID(string otherFriend)
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
