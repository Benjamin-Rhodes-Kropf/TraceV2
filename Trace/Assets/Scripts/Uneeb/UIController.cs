using System.Collections;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.XR.ARFoundation.Samples;

public class UIController : PressInputBase
{

    public ToggleCameraFacingDirectionOnPress cameraSwitch;
    public VideoPlayer previewVideoPlayer;
    
    bool isVideoPlayerOpenedForRestingTheSceneToClearGarbageValues = false;
    bool isImagePreviewOpenedForRestingTheSceneToClearGarbageValues = false;
    public CameraManager camManger;
    // Start is called before the first frame update
    void Start()
    {
        ScreenManager.instance.mainCanvas.SetActive(false);
    }

    public void CloseVideoPreview() {
        // app is reopened after minimising then reload the scene and close the video player and panel
        if (isVideoPlayerOpenedForRestingTheSceneToClearGarbageValues)
        {
            camManger.videoPreviewPanel.SetActive(false);
            previewVideoPlayer.gameObject.SetActive(false);
            camManger.cameraPanel.SetActive(true);
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
        // this will simply close the panel
        else
        {
            camManger.videoPreviewPanel.SetActive(false);
            previewVideoPlayer.gameObject.SetActive(false);
            camManger.cameraPanel.SetActive(true);
        }
    }
    //for switching the camera
    public void SwitchCamera() {
        cameraSwitch.ToggleCamera();
    }
    //It will close the image previewer
    public void CloseImagePreview()
    {
        // app is reopened after minimising then reload the scene and close the panel
        if (isImagePreviewOpenedForRestingTheSceneToClearGarbageValues)
        {
            //SceneManager.LoadScene(0);
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            camManger.imagePreviewPanel.gameObject.SetActive(false);
            camManger.cameraPanel.SetActive(true);
            //ScreenManager.instance.ChangeScreenNoAnim("HomeScreen");
        }
        // this will simply close the panel
        else
        {
            camManger.imagePreviewPanel.gameObject.SetActive(false);
            camManger.cameraPanel.SetActive(true);
        }
    }
    //write sharing code here
    public void Share() {
        Debug.Log("Pass Video To Firebase Manager Here");
    }
    public void ShowImagePreview(string path) {
        StartCoroutine(path);
    }

    [System.Obsolete]
    IEnumerator LoadingImages(string path)
    {
        //yield return new WaitForSeconds(1f);
        using (UnityWebRequest uwr = UnityWebRequestTexture.GetTexture(path))
        {
            yield return uwr.SendWebRequest();

            if (uwr.isNetworkError || uwr.isHttpError)
            {
                Debug.Log(uwr.error);
            }
            else
            {
                // Get downloaded asset bundle
                var texture = DownloadHandlerTexture.GetContent(uwr);
                Texture2D nexTexture = new Texture2D(texture.width, texture.height);
                nexTexture.LoadImage(uwr.downloadHandler.data);
                camManger.previewImagePlayer.sprite = Sprite.Create(nexTexture, new Rect(0, 0, nexTexture.width, nexTexture.height), new Vector2(0, 0));
                camManger.previewImagePlayer.color = Color.white;
            }
        }
    }
    //It will capture the image
    public void CaputureImage()
    {
        //turning off the UI so that i won't visible in image.
        camManger.cameraPanel.SetActive(false);
        StartCoroutine(RecordFrame());

    }
    IEnumerator RecordFrame()
    {
        yield return new WaitForEndOfFrame();
        var texture = ScreenCapture.CaptureScreenshotAsTexture();
        // do something with texture
        //Show the image captured
        camManger.imagePreview.texture = texture;
        camManger.imagePreviewPanel.SetActive(true);
        camManger.videoPreviewPanel.SetActive(false);

        // cleanup
        //Object.Destroy(texture);
    }

    //This will reload the AR camera scene so that garbage values can be clean,
    //which was causing jitter in audio after reopening the app on minimising
    private void OnApplicationFocus(bool focus)
    {

        //Check if video was playing or not, if no then reload the camera scene
        Debug.Log(">>>>> Application Focus Status is " + focus + " <<<<<");
        if (focus && (!camManger.videoPreviewPanel.activeInHierarchy && !camManger.imagePreviewPanel.activeInHierarchy))
        {
            Debug.Log("=========  Scene Reloaded on Focus Changed" + "  =========");
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
        //if video player or image previewer was active last time then some flag
        //values will be set to reset the scenes on closing of previewers
        else if (focus && (camManger.videoPreviewPanel.activeInHierarchy || camManger.imagePreviewPanel.activeInHierarchy))
        {
            //if video player is active
            if (camManger.videoPreviewPanel.activeInHierarchy)
            {
                isVideoPlayerOpenedForRestingTheSceneToClearGarbageValues = true;
                Debug.Log(">>>>> Video Player is active and scene will be restrated on closing of player <<<<<");
                previewVideoPlayer.Play();
            }
            //if image previewer is active
            else if (camManger.imagePreviewPanel.activeInHierarchy)
            {
                isImagePreviewOpenedForRestingTheSceneToClearGarbageValues = true;
                Debug.Log(">>>>> Image Viewer is active and scene will be restrated on closing of viewer <<<<<");
            }
        }
    }
}