using System;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.XR.ARFoundation.Samples;

public class CameraManager : MonoBehaviour//PressInputBase
{
    public GameObject cameraPanel;
    public GameObject videoPreviewPanel;
    public GameObject imagePreviewPanel;
    public Image previewImagePlayer;
    public RawImage imagePreview;

    public UIController uiManager;
    void Awake()
    {
       
    }
    // Start is called before the first frame update
    void Start()
    {
    
    }
    public void SwitchCamera() {
        uiManager.SwitchCamera();
    }
    public void CloseVideoPreview()
    {
        uiManager.CloseVideoPreview();
    }
    public void CloseImagePreview()
    {
        uiManager.CloseImagePreview();

    }
    //write sharing code here
    public void ShareVideo()
    {
        Debug.Log("Pass Video To Firebase Manager Here");
    }
    public void ShareImage()
    {
        Debug.Log("Pass Image To Firebase Manager Here");
    }

    public void CaputureImage()
    {
        uiManager.CaputureImage();
    }
    
}