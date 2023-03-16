using System;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CameraManager : MonoBehaviour//PressInputBase
{
    public ReplayCam replayCamera;
    public GameObject cameraPanel;
    public GameObject videoPreviewPanel;
    public GameObject imagePreviewPanel;
    public Image previewImagePlayer;
    public RawImage imagePreview;

    //public UIController uiManager;
    private void OnEnable()
    {
        cameraPanel.SetActive(true);
    }

    //for switching between the device cameras
    public void SwitchCamera() {
        ScreenManager.instance.uiController.SwitchCamera();
    }
    //closing the video preview
    public void CloseVideoPreview()
    {
        ScreenManager.instance.uiController.CloseVideoPreview();
    }
    //closing the video Image preview
    public void CloseImagePreview()
    {
        ScreenManager.instance.uiController.CloseImagePreview();

    }
    public void SaveVideo()
    {
        ScreenManager.instance.uiController.SaveVideo();
    }
    public void BackToMainScene() {
        //change the bool so that the main canavs can be enabled after the main scene is loaded
        ScreenManager.instance.isComingFromCameraScene = true;
        SceneManager.LoadScene(0);
        ScreenManager.instance.camManager.cameraPanel.SetActive(false);//disabling the camera panel
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
    //For capturing the image
    public void CaputureImage()
    {
        ScreenManager.instance.uiController.CaputureImage();
    }
    //This was used for testing
    public void StartRecording() {
        ScreenManager.instance.uiController.vidRecorder.StartRecording();
    }
    //This was used for testing
    public void StopRecording() {
        ScreenManager.instance.uiController.vidRecorder.StopRecording();

    }

}