using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CameraManager : MonoBehaviour//PressInputBase
{
    public GameObject cameraPanel;
    public GameObject videoPreviewPanel;
    public GameObject imagePreviewPanel;
    public Image previewImagePlayer;
    public RawImage imagePreview;

    public UIController uiManager;

    //for switching between the device cameras
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

    public void CaputureImage()
    {
        uiManager.CaputureImage();
    }
    
}