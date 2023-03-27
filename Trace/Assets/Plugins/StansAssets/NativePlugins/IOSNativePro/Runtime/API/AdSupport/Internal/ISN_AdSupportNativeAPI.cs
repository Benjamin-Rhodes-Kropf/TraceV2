using System;
using UnityEngine; 
using SA.iOS.Utilities; 

#if UNITY_IPHONE
using System.Runtime.InteropServices;
#endif

namespace SA.iOS.AdSupport.Internal 
{ 
    /// <summary> 
    /// This is api for getting data from native iOS 
    /// </summary> 
    internal class ISN_AdSupportNativeAPI 
    { 
        #if UNITY_IPHONE && AS_SUPPORT_API_ENABLED 
        [DllImport("__Internal")] private static extern string _ISN_GetAdvertisingIdentifier(); 
        [DllImport("__Internal")] private static extern bool _ISN_AdvertisingTrackingEnabled(); 
        #endif 
        

        /// <summary>
        /// Get AdvertisingIdentifier from ASIdentifierManager native api.
        /// </summary>
        internal static string AdvertisingIdentifier
        {
            get
            {
                string m_Identifier = null;

                #if UNITY_IPHONE && !UNITY_EDITOR && AS_SUPPORT_API_ENABLED
                m_Identifier = _ISN_GetAdvertisingIdentifier();
                #endif

                return m_Identifier;
            }
        }
        
        /// <summary>
        /// Get AdvertisingTrackingEnabled from ASIdentifierManager 
        /// value that indicates whether the user has limited ad tracking.
        /// </summary>
        internal static bool AdvertisingTrackingEnabled
        {
            get
            {
                bool m_TrackingEnabled = false;

                #if UNITY_IPHONE && !UNITY_EDITOR && AS_SUPPORT_API_ENABLED
                m_TrackingEnabled = _ISN_AdvertisingTrackingEnabled();
                #endif

                return m_TrackingEnabled;
            }
        }
    } 
        
}