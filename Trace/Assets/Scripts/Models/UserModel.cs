using System;
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
    public string Password;
    public bool isLoggedIn = false;

    private Sprite profilePicture = null;
    public void ProfilePicture(Action<Sprite> callback)
    {
        if (profilePicture == null)
        {
            DownloadProfilePicture((sprite =>
            {
                profilePicture = sprite;
                callback(profilePicture);
            }));
        }
        else
        {
            callback(profilePicture);
        }
        
    }

    public void DownloadProfilePicture(Action<Sprite> callback)
    {
        FbManager.instance.GetProfilePhotoFromFirebaseStorage(userId, (tex) =>
        {
           profilePicture = Sprite.Create(ChangeTextureType(tex), new Rect(0.0f, 0.0f, tex.width, tex.height), new Vector2(0.5f, 0.5f), 100.0f);;
           callback(profilePicture);
        }, (message) =>
        {
            Debug.Log(message);
        });
    }


    private Texture2D ChangeTextureType(Texture texture)
    {
        return Texture2D.CreateExternalTexture(
            texture.width,
            texture.height,
            TextureFormat.RGB24,
            false, false,
            texture.GetNativeTexturePtr());
    }

    public UserModel(string _userId, string email, int friendCount, string displayName, string username, string phoneNumber, string photoURL, string password = "", bool isLoggedIn = false)
    {
        this.userId = _userId;
        this.Email = email;
        this.FriendCount = friendCount;
        this.DisplayName = displayName;
        this.Username = username;
        this.PhoneNumber = phoneNumber;
        this.PhotoURL = photoURL;
        this.Password = password;
        this.isLoggedIn = isLoggedIn;
    }
}
