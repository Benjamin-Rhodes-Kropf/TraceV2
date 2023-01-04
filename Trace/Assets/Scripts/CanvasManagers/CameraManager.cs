using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class CameraManager : MonoBehaviour
{
    //test
    [SerializeField] private bool hasBeenActivated = false;
    // Declare a WebCamTexture to store the webcam video feed
    private WebCamTexture webcamTexture;

    // Declare a RawImage to display the webcamTexture on a UI panel
    public RawImage image;

    void OnEnable()
    {
        //dosnt active camera on startup of app
        if (!hasBeenActivated)
        {
            hasBeenActivated = true;
            return;
        }
        
        Debug.Log("Camera: Camera is Being Activated");

        // Get the default webcam
        WebCamDevice[] devices = WebCamTexture.devices;
        webcamTexture = new WebCamTexture(devices[0].name);

        // Apply the webcamTexture to the RawImage's texture
        image.texture = webcamTexture;

        // Flip the texture horizontally by setting the x coordinate of the uvRect to 1
        image.uvRect = new Rect(1, 0, -1, 1);

        // Start playing the webcam video feed
        webcamTexture.Play();

        // Set the size of the RawImage to match the size of the webcamTexture
        // rawImage.SetNativeSize();
        if (Screen.width > Screen.height)
        {
            image.rectTransform.sizeDelta = new Vector2(Screen.height, Screen.height);
        }
        else
        {
            image.rectTransform.sizeDelta = new Vector2(Screen.width, Screen.width);
        }
        
        //Add Mesh Renderer to the GameObject to which this script is attached to
        image.GetComponent<Renderer>().material.mainTexture = webcamTexture;
    }

    public void CaptureImage()
    {
        Debug.Log("Camera: CaptureImage");
        StartCoroutine(TakePhoto());
    }

    void SaveTextureToFile (Texture2D texture, string filename) { 
        System.IO.File.WriteAllBytes (filename, texture.EncodeToPNG());
    }
    
    IEnumerator TakePhoto() // Start this Coroutine on some button click
    {
        // NOTE - you almost certainly have to do this here:

        yield return new WaitForSeconds(0.01f);
        Debug.Log("Camera: waited for seconds");

        // it's a rare case where the Unity doco is pretty clear,
        // http://docs.unity3d.com/ScriptReference/WaitForEndOfFrame.html
        // be sure to scroll down to the SECOND long example on that doco page 

        Texture2D photo = new Texture2D(webcamTexture.width, webcamTexture.height);
        photo.SetPixels(webcamTexture.GetPixels());
        photo.Apply();
        
        //Encode to a PNG
        byte[] bytes = photo.EncodeToPNG();
        
        
        //Write out the PNG. Of course you have to substitute your_path for something sensible
        File.WriteAllBytes("Assets/" + "photo.png", bytes);
        
        webcamTexture.Stop();
        Debug.Log("Camera: webcam texture stop");
        ScreenManager.instance.ChangeScreenNoAnim("View Capture");
    }
}