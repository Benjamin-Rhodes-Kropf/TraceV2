using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TraceUserInfoStructure {
    //they must be public for json utility conversion
    public string username;
    public string name;
    public string email;
    public string phone;
    public string userPhotoUrl;
    public bool isOnline;
    public int friendCount;
    public int score;
    public bool isLogedIn;
    
    public TraceUserInfoStructure()
    {
        isOnline = false;
        isLogedIn = true;
        friendCount = 0;
        score = 0;
    }

    public TraceUserInfoStructure(string username, string name, string userPhotoLink, string email, string phone) {
        this.username = username;
        this.name = name;
        this.userPhotoUrl = userPhotoLink;
        this.email = email;
        this.phone = phone;
        isLogedIn = true;
    }
}
