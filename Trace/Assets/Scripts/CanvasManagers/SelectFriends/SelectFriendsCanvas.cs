using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SelectFriendsCanvas : MonoBehaviour
{
    private void OnEnable()
    {
        if(ScreenManager.instance.uiController!=null)
            ScreenManager.instance.uiController.previewVideoPlayer.gameObject.SetActive(false);//disabling the camera panel
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void TurnOffCamera() {
        if (ScreenManager.instance.uiController != null)
        {
            ScreenManager.instance.camManager.cameraPanel.SetActive(false);//disabling the camera panel
            ScreenManager.instance.uiController.previewVideoPlayer.gameObject.SetActive(false);//disabling the camera panel
        }
    }
}
