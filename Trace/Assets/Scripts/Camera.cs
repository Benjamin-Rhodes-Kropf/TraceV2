using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Camera : MonoBehaviour
{
    // Declare a WebCamTexture to store the webcam video feed
    private WebCamTexture webcamTexture;

    // Declare a RawImage to display the webcamTexture on a UI panel
    public RawImage image;
    
    void OnEnable()
    {
        //make sure the game object is active
        if (!gameObject.activeInHierarchy)
        {
            return;
        }
        
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
    }
}
