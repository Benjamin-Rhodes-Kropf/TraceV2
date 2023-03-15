namespace UnityEngine.XR.ARFoundation.Samples
{
    public class ToggleCameraFacingDirectionOnPress : PressInputBase
    {
        [SerializeField]
        ARCameraManager m_CameraManager;
        bool flag = true;

        public ARCameraManager cameraManager
        {
            get => m_CameraDirection.cameraManager;
            set => m_CameraDirection.cameraManager = value;
        }

        CameraDirection m_CameraDirection;

        protected override void Awake()
        {
            base.Awake();
            m_CameraDirection = new CameraDirection(m_CameraManager);
        }
        //protected override void OnPressBegan(Vector3 position)
        //{
        //    m_CameraDirection.Toggle();
        //}
        public void ToggleCamera() {
            m_CameraDirection.Toggle();

        }
        
        public static bool DoubleTap
        {
            get { return Input.touchSupported && (Input.touches.Length > 0) && (Input.touches[0].tapCount == 2); }
        }
        void Update()
        {
            if (DoubleTap && flag)
            {
                flag = false;
                ToggleCamera();
                Debug.Log(">>> Double Tap Detected <<<");
                Invoke("ResetFlagValue", 1f);
            }
        }
        void ResetFlagValue()
        {
            flag = true;
        }

    }
}
