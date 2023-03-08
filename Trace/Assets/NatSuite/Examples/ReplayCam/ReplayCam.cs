/* 
*   NatCorder
*   Copyright (c) 2021 Yusuf Olokoba
*/

namespace NatSuite.Examples
{
#if UNITY_EDITOR
    using UnityEditor;
#endif
    using System.Collections;
    using UnityEngine;
    using Recorders;
    using Recorders.Clocks;
    using Recorders.Inputs;
    using UnityEngine.Video;
    using UnityEngine.UI;
    public class ReplayCam : MonoBehaviour
    {

        [Header(@"Recording")]
        public int videoWidth = 720;
        public int videoHeight = 1280;

        [Header("Microphone")]
        public bool recordMicrophone;
        private AudioSource microphoneSource;

        private MP4Recorder videoRecorder;
        private IClock recordingClock;
        private CameraInput cameraInput;
        private AudioInput audioInput;
        public CameraManager camManager;
        //public UIController uiManager;

        private MP4Recorder recorder;
        //private CameraInput cameraInput;
        //private AudioInput audioInput;
        //public VideoPlayer vPlayer;
        //public UIController uIManager;
        Color32[] pixelArray;

       
        private IEnumerator Start()
        {
            
            // Start microphone
            microphoneSource = gameObject.GetComponent<AudioSource>();
            microphoneSource.mute = false;
            microphoneSource.loop = true;
            microphoneSource.bypassEffects =
            microphoneSource.bypassListenerEffects = false;
            microphoneSource.clip = Microphone.Start(null, true, 1, AudioSettings.outputSampleRate);
            yield return new WaitUntil(() => Microphone.GetPosition(null) > 0);
            //microphoneSource.Play();
        }

        private void OnDestroy()
        {
            // Stop microphone
            if (microphoneSource != null)
            {
                microphoneSource.Stop();
                Microphone.End(null);
            }
        }
        void Update()
        {
            //// GOOD // Copy into the same array first
            //cameraInput..GetPixels32(pixelArray);
            //// Commit that array
            //recorder.CommitFrame(pixelArray);
        }
        public void StartRecording()
        {
            // Start recording
            microphoneSource.Play();
            var frameRate = 30;
            var sampleRate = recordMicrophone ? AudioSettings.outputSampleRate : 0;
            var channelCount = recordMicrophone ? (int)AudioSettings.speakerMode : 0;
            var clock = new RealtimeClock();
            recorder = new MP4Recorder(videoWidth, videoHeight, frameRate, sampleRate, channelCount, audioBitRate: 96_000);
            // Create recording inputs
            cameraInput = new CameraInput(recorder, clock, Camera.main);
            audioInput = recordMicrophone ? new AudioInput(recorder, clock, microphoneSource, true) : null;
            // Unmute microphone
            microphoneSource.mute = audioInput == null;

        }

        public async void StopRecording()
        {
            // Mute microphone
            microphoneSource.mute = true;
            // Stop recording
            audioInput?.Dispose();
            cameraInput.Dispose();
            microphoneSource.Stop();
            var path = await recorder.FinishWriting();
            // Playback recording via unity player
            Debug.Log($"Saved recording to: {path}");
            //string imgName = "VID_" + System.DateTime.Now.ToString("yyyymmdd_HHmmss") + ".mp4";
            //NativeGallery.Permission permission = NativeGallery.SaveVideoToGallery(path, "TraceVideo", imgName, null);
            //Debug.Log("Permission result: " + permission);
            camManager.cameraPanel.SetActive(false);
            camManager.videoPreviewPanel.SetActive(true);
            camManager.uiManager.previewVideoPlayer.gameObject.SetActive(true);
            camManager.uiManager.previewVideoPlayer.url = path;
            camManager.uiManager.previewVideoPlayer.Play();
            //NativeGallery.SaveVideoToGallery(path,"Trace",)
            //Handheld.PlayFullScreenMovie($"file://{path}");
        }
    }
}