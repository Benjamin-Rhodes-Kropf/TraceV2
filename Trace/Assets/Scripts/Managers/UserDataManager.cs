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
        foreach (var request in FbManager.instance._allRequests)
        {
                var _userSearchQuery =
                from user in FbManager.instance.AllUsers
                where string.Equals(user.userId, request.SenderID, StringComparison.Ordinal)
                select user;
                
                users.AddRange(_userSearchQuery.ToArray());
        }
        return users;
    }
    
    
    public List<UserModel> GetAllFriends()
    {
        List<UserModel> users = new List<UserModel>();
        foreach (var request in FbManager.instance._allFriends)
        {
            var _userSearchQuery =
                from user in FbManager.instance.AllUsers
                where string.Equals(user.userId, request.friend2, StringComparison.Ordinal)
                select user;
                
            users.AddRange(_userSearchQuery.ToArray());
        }
        return users;
    }
    
}
