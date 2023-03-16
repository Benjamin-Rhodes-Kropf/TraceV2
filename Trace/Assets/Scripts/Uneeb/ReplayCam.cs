/* 
*   NatCorder
*   Copyright (c) 2020 Yusuf Olokoba
*/
using UnityEngine;
using NatML.VideoKit;

public class ReplayCam : MonoBehaviour
{

    public VideoKitRecorder vidRecorder;
    public void StartRecording()
    {
        vidRecorder.StartRecording();
        Debug.Log("Recording Start");

    }

    public void StopRecording()
    {
        vidRecorder.StopRecording();
        Debug.Log("Recording Stoped");
    }
}