using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class UserDataManager
{
    private static UserDataManager instance = null;

    public static UserDataManager Instance
    {
        get
        {
            if (instance == null)
                instance = new UserDataManager();

            return instance;
        }
    }


    private UserDataManager()
    {
        
    }
    
    public List<UserModel> GetUsersByLetters(string name)
    {
        List<UserModel> selectedUsers = new List<UserModel>();

        // Query Syntax
        IEnumerable<UserModel> _userSearchQuery =
            from user in FbManager.instance.AllUsers
            where user.Username.Contains(name)
            orderby user.Username
            select user;
        
        selectedUsers.AddRange(_userSearchQuery);

        return selectedUsers;
    }

    

    public bool IsUsernameAvailable(string userName)
    {
        var users = GetUsersByLetters(userName);
        return users.Count < 1;
    }


    public List<UserModel> GetFriendRequested()
    {
        List<UserModel> users = new List<UserModel>();
        foreach (var request in FbManager.instance._allReceivedRequests)
        {
                var _userSearchQuery =
                from user in FbManager.instance.AllUsers
                where string.Equals(user.userId, request.SenderID)
                select user;
                users.AddRange(_userSearchQuery.ToArray());
        }
        return users;
    }
    
    public List<UserModel> GetSentFriendRequests()
    {
        List<UserModel> users = new List<UserModel>();
        foreach (var request in FbManager.instance._allSentRequests)
        {
            var _userSearchQuery =
                from user in FbManager.instance.AllUsers
                where string.Equals(user.userId, request.ReceiverId, StringComparison.Ordinal)
                select user;
                
            users.AddRange(_userSearchQuery.ToArray());
        }
        return users;
    }
    public List<UserModel> GetRequestsByName(string name, bool isReceived =  true)
    {
        var users = isReceived ? GetFriendRequested() : GetSentFriendRequests();
        List<UserModel> selectedUsers = new List<UserModel>();
        if (string.IsNullOrEmpty(name) is false && users.Count > 0)
        {
            // Query Syntax
            IEnumerable<UserModel> _userSearchQuery =
                from user in users
                where user.Username.Contains(name)
                orderby user.Username
                select user;
        
            selectedUsers.AddRange(_userSearchQuery);
        }
        return selectedUsers;
    }
    
    public List<UserModel> GetAllFriends()
    {
        List<UserModel> users = new List<UserModel>();
        foreach (var friendModel in FbManager.instance._allFriends)
        {
            var _userSearchQuery =
                from user in FbManager.instance.AllUsers
                where string.Equals(user.userId, friendModel.friend, StringComparison.Ordinal)
                select user;
                
            users.AddRange(_userSearchQuery.ToArray());
        }
        return users;
    }

    public List<UserModel> GetFriendsByName(string name)
    {
        var users = GetAllFriends();
            List<UserModel> selectedUsers = new List<UserModel>();
        if (string.IsNullOrEmpty(name) is false && users.Count > 0)
        {
            // Query Syntax
            IEnumerable<UserModel> _userSearchQuery =
                from user in users
                where user.Username.Contains(name)
                orderby user.Username
                select user;
        
            selectedUsers.AddRange(_userSearchQuery);
        }
        return selectedUsers;
    }

    public void GetAllUsersBySearch(string name, out List<UserModel> friends, out List<UserModel> requests, out List<UserModel> requestsSent, out List<UserModel> others)
    {
        friends = new List<UserModel>();
        requests = new List<UserModel>();
        requestsSent = new List<UserModel>();
        others = new List<UserModel>();

        friends = GetFriendsByName(name);
        requests = GetRequestsByName(name);
        requestsSent = GetRequestsByName(name, false);
        others = GetUsersByLetters(name);
    }
    
}
