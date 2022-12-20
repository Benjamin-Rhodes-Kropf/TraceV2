using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TraceUser {
    //they must be public for json utility conversion
    public string username;
    public string name;
    public string email;
    public string phone;
    public string userPhotoUrl;
    public bool isOnline;
    public int friendCount;
    public int score;

    
    public TraceUser()
    {
        isOnline = false;
        friendCount = 0;
        score = 0;
    }

    public TraceUser(string username, string name, string userPhotoLink, string email, string phone) {
        this.username = username;
        this.name = name;
        this.userPhotoUrl = userPhotoLink;
        this.email = email;
        this.phone = phone;
    }
}
