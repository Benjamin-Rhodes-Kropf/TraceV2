using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditProfileCanvas : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }
    
    void SendEmail ()
    {
        string email = "me@example.com";
        string subject = MyEscapeURL("My Subject");
        string body = MyEscapeURL("My Body\r\nFull of non-escaped chars");
        Application.OpenURL("mailto:" + email + "?subject=" + subject + "&body=" + body);
    }
    string MyEscapeURL (string url)
    {
        return WWW.EscapeURL(url).Replace("+","%20");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
