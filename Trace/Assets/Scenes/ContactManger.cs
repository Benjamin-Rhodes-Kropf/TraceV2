using System.Collections;
using System.Collections.Generic;
using SA.iOS.Contacts;
using UnityEngine;


public class ContactManger : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Attempting To Get Contanct Information");
        var status = ISN_CNContactStore.GetAuthorizationStatus(ISN_CNEntityType.Contacts);
        if(status == ISN_CNAuthorizationStatus.Authorized) {
            Debug.Log("Contacts Permission granted");
        } else {
            ISN_CNContactStore.RequestAccess(ISN_CNEntityType.Contacts, (result) => {
                if (result.IsSucceeded) {
                    Debug.Log("Contacts Permission granted");
                } else {
                    Debug.Log("Contacts Permission denied");
                }
            });
        }
        
        ISN_CNContactStore.ShowContactsPickerUI((result) => {
            if (result.IsSucceeded) {
                foreach (var contact in result.Contacts) {
                    Debug.Log(contact.GivenName);
                }
            } else {
                Debug.Log("Error: " + result.Error.Message);
            }
        });
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
