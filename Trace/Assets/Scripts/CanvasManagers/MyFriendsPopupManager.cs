using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyFriendsPopupManager : MonoBehaviour
{
    [SerializeField] private ScrollContentSizer _scrollContentSizer;
    [SerializeField] private GameObject _displayFriendPanel;
    private void OnEnable()
    {
        //Todo:get users friends from the firebase manager
        //Todo:set _scrollContentSizer height based on number of friends
        //Todo:instantiate friend panels based on number of friends with correct content
        Debug.Log("MyFriendsPopupManager: Enabled");
    }
    private void OnDisable()
    {
        Debug.Log("MyFriendsPopupManager: Disabled");
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
