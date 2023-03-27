using System;
using UnityEngine;
using SA.iOS.Utilities;
using SA.Foundation.Templates;

namespace SA.iOS.EventKit
{
    
    /// <summary>
    /// EventKit saving result that contains result of saving
    /// events or reminders and their identifier.
    /// </summary>
    [Serializable]
    public class ISN_EventKitSaveResult 
    {
        [SerializeField] string m_Identifier = null;
        [SerializeField] SA_Result m_Result = null;


        /// <summary>
        /// Events or reminder identifier in EventKit.
        /// Need for removing or updating them.
        /// </summary>
        public string Identifier
        {
            get
            {
                return m_Identifier;
            }
        }

        /// <summary>
        /// Result of saving events and reminders by using EventKit.
        /// </summary>
        public SA_Result Result
        {
            get
            {
                return m_Result;
            }
        }
    }
}
