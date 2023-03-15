using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserModel
{
    public string Email;
    public int FriendCount;
    public string DisplayName;
    public string Username;
    public string PhoneNumber;
    public string PhotoURL;
    
    public UserModel(string email, int friendCount, string displayName, string username, string phoneNumber, string photoURL)
    {
        this.Email = email;
        this.FriendCount = friendCount;
        this.DisplayName = displayName;
        this.Username = username;
        this.PhoneNumber = phoneNumber;
        this.PhotoURL = photoURL;
    }
}
