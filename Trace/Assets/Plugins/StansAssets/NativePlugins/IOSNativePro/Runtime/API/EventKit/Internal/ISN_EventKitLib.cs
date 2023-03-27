
namespace SA.iOS.EventKit.Internal
{
    internal static class ISN_EventKitLib
    {
        private static ISN_EventKitAPI m_api = null;
        public static ISN_EventKitAPI API 
        {
            get 
            {
                if (m_api == null) 
                {
                    m_api = ISN_EventKitNativeAPI.Instance;
                }

                return m_api;
            }
        }
    }
}
