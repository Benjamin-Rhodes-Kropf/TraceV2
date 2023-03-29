using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using NatML.VideoKit;
using static NatML.VideoKit.VideoKitRecorder;

public class UIController : MonoBehaviour
{
    public VideoKitAudioManager microPhoneAudioKit;
    public VideoPlayer previewVideoPlayer;
    public VideoKitCameraManager swapCamera;
    public VideoKitRecorder vidRecorder;
    bool isVideoPlayerOpenedForRestingTheSceneToClearGarbageValues = false;
    bool isImagePreviewOpenedForRestingTheSceneToClearGarbageValues = false;
    public CameraManager camManger;
    [SerializeField] private string path;
    public GameObject cameraView;

    // Start is called before the first frame update
    void Start()
    {
        //assigning the ui controller in screenamanet script
        ScreenManager.instance.uiController = this;
        //assigning the camera maanager from screenamanet script and other vaiables
        camManger = ScreenManager.instance.camManager;
        camManger.replayCamera.vidRecorder = vidRecorder;
        //To reduce the vide size we set the video bit rate to half,
        vidRecorder.videoBitRate = 10000000 / 2;
        //To show microphone prompt to avoid a conflict which occurs once in start
        microPhoneAudioKit.StartRunning();
    }

    public void CloseVideoPreview() {
        //// this will close the panel preview panel and reopen the camera panel
        camManger.videoPreviewPanel.SetActive(false);
        previewVideoPlayer.gameObject.SetActive(false);
        camManger.cameraPanel.SetActive(true);
        cameraView.SetActive(true);
    }
    //for switching the camera
    public void SwitchCamera() {
        if (swapCamera.facing == VideoKitCameraManager.Facing.PreferUser)
        {
            swapCamera.facing = VideoKitCameraManager.Facing.PreferWorld;
        }
        else
        {
            swapCamera.facing = VideoKitCameraManager.Facing.PreferUser;

        }
    }
    //It will close the image previewer
    public void CloseImagePreview()
    {
        //// this will close the panel preview panel and reopen the camera panel
        camManger.imagePreviewPanel.gameObject.SetActive(false);
        camManger.cameraPanel.SetActive(true);
        cameraView.SetActive(true);
    }
    //write sharing code here
    public void ShareVideo() {
        Debug.Log("Pass Video To Firebase Manager Here");
        StartCoroutine(FbManager.instance.UploadTraceVideo(path));
    }

    public void ShareImage()
    {
    #if UNITY_EDITOR
        StartCoroutine(FbManager.instance.UploadTraceImage( Application.dataPath + "/SaveImages/Traces/Image.png"));
#elif UNITY_IPHONE
        StartCoroutine(FbManager.instance.UploadTraceImage( Application.persistentDataPath + "/SaveImages/Traces/Image.png"));
#endif
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
        
        //Show the image captured
        camManger.imagePreview.texture = texture;
        camManger.imagePreviewPanel.SetActive(true);
        camManger.videoPreviewPanel.SetActive(false);
        cameraView.SetActive(false);
        
        //SAVE IMAGE TO DEVICE STORAGE
        byte[] bytes = texture.EncodeToPNG();
        var dirPath = "";

        #if UNITY_EDITOR
        dirPath = Application.dataPath + "/SaveImages/Traces/";
        #elif UNITY_IPHONE
         dirPath = Application.persistentDataPath + "/SaveImages/Traces/";
        #endif 
        
        if(!Directory.Exists(dirPath)) {
            Directory.CreateDirectory(dirPath);
        }
        File.WriteAllBytes(dirPath + "Image" + ".png", bytes);
        Debug.Log("file location:" + dirPath + "Image" + ".png");
        
        //cleanup
        //Object.Destroy(texture);
    }
    
    //This is an event which is handle the video preview work afetr the recording is done
    public void OnRecordingCompleted(RecordingSession session)
    {
        // Get the recording path
        path = session.path;
        Debug.Log(path+" This is the temp path");
        //setting the video player file target path
        previewVideoPlayer.url = path;
        //enabling the videoplayer
        previewVideoPlayer.gameObject.SetActive(true);
        //diabling the camera panel
        camManger.cameraPanel.SetActive(false);
        //enabling the video preview panel UI
        camManger.videoPreviewPanel.SetActive(true);
        //playing the video on video player
        previewVideoPlayer.Play();
        //disabling the camera view
        cameraView.SetActive(false);

    }
    //To save the video in mobile gallery
    public void SaveVideo() {
        string imgName = "VID_" + System.DateTime.Now.ToString("yyyymmdd_HHmmss") + ".mp4";
        NativeGallery.Permission permission = NativeGallery.SaveVideoToGallery(path, "TraceVideo", imgName, null);
        Debug.Log("Permission result: " + permission);
    }
    #region
    //This will reload the AR camera scene so that garbage values can be clean,
    //which was causing jitter in audio after reopening the app on minimising
    private void OnApplicationFocus(bool focus)
    {

        ////Check if video was playing or not, if no then reload the camera scene
        //Debug.Log(">>>>> Application Focus Status is " + focus + " <<<<<");
        //if (focus && (!camManger.videoPreviewPanel.activeInHierarchy && !camManger.imagePreviewPanel.activeInHierarchy))
        //{
        //    Debug.Log("=========  Scene Reloaded on Focus Changed" + "  =========");
        //    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        //}
        ////if video player or image previewer was active last time then some flag
        ////values will be set to reset the scenes on closing of previewers
        //else if (focus && (camManger.videoPreviewPanel.activeInHierarchy || camManger.imagePreviewPanel.activeInHierarchy))
        //{
        //    //if video player is active
        //    if (camManger.videoPreviewPanel.activeInHierarchy)
        //    {
        //        isVideoPlayerOpenedForRestingTheSceneToClearGarbageValues = true;
        //        Debug.Log(">>>>> Video Player is active and scene will be restrated on closing of player <<<<<");
        //        previewVideoPlayer.Play();
        //    }
        //    //if image previewer is active
        //    else if (camManger.imagePreviewPanel.activeInHierarchy)
        //    {
        //        isImagePreviewOpenedForRestingTheSceneToClearGarbageValues = true;
        //        Debug.Log(">>>>> Image Viewer is active and scene will be restrated on closing of viewer <<<<<");
        //    }
        //}
    }
    #endregion
}