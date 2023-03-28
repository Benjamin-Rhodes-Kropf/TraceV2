using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserModel
{
    public string userId; 
    public string Email;
    public int FriendCount;
    public string DisplayName;
    public string Username;
    public string PhoneNumber;
    public string PhotoURL;
    
    public UserModel(string _userId, string email, int friendCount, string displayName, string username, string phoneNumber, string photoURL)
    {
        this.userId = _userId;
        this.Email = email;
        this.FriendCount = friendCount;
        this.DisplayName = displayName;
        this.Username = username;
        this.PhoneNumber = phoneNumber;
        this.PhotoURL = photoURL;
    }
}
