using System;
using UnityEngine; 
using SA.iOS.Utilities; 

namespace SA.iOS.AdSupport 
{
    /// <summary>
    /// An object containing an identifier to be used only for serving advertisements, 
    /// and a flag indicating whether a user has limited ad tracking. 
    /// <summary>
    public class ISN_ASIdentifierManager
    {
        private static ISN_ASIdentifierManager m_sharedManager = null;
        /// <summary>
        /// Returns the shared instance of the ASIdentifierManager class.
        /// </summary>
        public static ISN_ASIdentifierManager SharedManager
        {
            get
            {
                if(m_sharedManager == null)
                {
                    m_sharedManager = new ISN_ASIdentifierManager();
                }
                return m_sharedManager;
            }
        }

        /// <summary>
        /// An alphanumeric string unique to each device, used only for serving advertisements.
        /// </summary>
        public string AdvertisingIdentifier
        {
            get
            {
                string m_Identifier = Internal.ISN_AdSupportNativeAPI.AdvertisingIdentifier;
                return m_Identifier;
            }
        }
        
        /// <summary>
        /// A Boolean value that indicates whether the user has limited ad tracking.
        /// </summary>
        public bool AdvertisingTrackingEnabled
        {
            get
            {
                bool m_TrackingEnabled = Internal.ISN_AdSupportNativeAPI.AdvertisingTrackingEnabled;
                return m_TrackingEnabled;
            }
        }
    }
}