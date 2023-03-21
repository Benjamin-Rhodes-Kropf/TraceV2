/* 
*   NatCorder
*   Copyright (c) 2021 Yusuf Olokoba.
*/

namespace NatSuite.Recorders.Internal {

    using AOT;
    using System;
    using System.Runtime.InteropServices;
    using System.Threading.Tasks;

    public abstract class NativeRecorder : IMediaRecorder {
        
        #region --Client API--
        /// <summary>
        /// Video size.
        /// </summary>
        public virtual (int width, int height) frameSize {
            get {
                recorder.FrameSize(out var width, out var height);
                return (width, height);
            }
        }

        /// <summary>
        /// Commit a video pixel buffer for encoding.
        /// The pixel buffer MUST have an RGBA8888 pixel layout.
        /// </summary>
        /// <param name="pixelBuffer">Pixel buffer to commit.</param>
        /// <param name="timestamp">Pixel buffer timestamp in nanoseconds.</param>
        public virtual unsafe void CommitFrame<T> (T[] pixelBuffer, long timestamp) where T : unmanaged {
            fixed (T* baseAddress = pixelBuffer)
                CommitFrame(baseAddress, timestamp);
        }

        /// <summary>
        /// Commit a video pixel buffer for encoding.
        /// The pixel buffer MUST have an RGBA8888 pixel layout.
        /// </summary>
        /// <param name="nativeBuffer">Pixel buffer in native memory to commit.</param>
        /// <param name="timestamp">Pixel buffer timestamp in nanoseconds.</param>
        public virtual unsafe void CommitFrame (void* nativeBuffer, long timestamp) => recorder.CommitFrame(nativeBuffer, timestamp);

        /// <summary>
        /// Commit an audio sample buffer for encoding.
        /// The sample buffer MUST be a linear PCM buffer interleaved by channel.
        /// </summary>
        /// <param name="sampleBuffer">Sample buffer to commit.</param>
        /// <param name="timestamp">Sample buffer timestamp in nanoseconds.</param>
        public virtual unsafe void CommitSamples (float[] sampleBuffer, long timestamp) {
            fixed (float* baseAddress = sampleBuffer)
                CommitSamples(baseAddress, sampleBuffer.Length, timestamp);
        }

        /// <summary>
        /// Commit an audio sample buffer for encoding.
        /// The sample buffer MUST be a linear PCM buffer interleaved by channel.
        /// </summary>
        /// <param name="nativeBuffer">Sample buffer in native memory to commit.</param>
        /// <param name="sampleCount">Total number of samples in the buffer.</param>
        /// <param name="timestamp">Sample buffer timestamp in nanoseconds.</param>
        public virtual unsafe void CommitSamples (float* nativeBuffer, int sampleCount, long timestamp) => recorder.CommitSamples(nativeBuffer, sampleCount, timestamp);

        /// <summary>
        /// Finish writing.
        /// </summary>
        /// <returns>Path to recorded media file.</returns>
        public virtual unsafe Task<string> FinishWriting () {
            var recordingTask = new TaskCompletionSource<string>();
            var handle = GCHandle.Alloc(recordingTask, GCHandleType.Normal);
            recorder.FinishWriting(OnRecording, (void*)(IntPtr)handle);
            return recordingTask.Task;
        }
        #endregion


        #region --Operations--

        private readonly IntPtr recorder;

        protected NativeRecorder (IntPtr recorder) => this.recorder = recorder;

        [MonoPInvokeCallback(typeof(Bridge.RecordingHandler))]
        private static unsafe void OnRecording (void* context, char* path) {
            // Get task
            var handle = (GCHandle)(IntPtr)context;
            var recordingTask = handle.Target as TaskCompletionSource<string>;
            handle.Free();
            // Complete task
            if (path != null)
                recordingTask.SetResult(Marshal.PtrToStringAnsi((IntPtr)path));
            else
                recordingTask.SetException(new Exception(@"Recorder failed to finish writing"));
        }
        #endregion
    }
}