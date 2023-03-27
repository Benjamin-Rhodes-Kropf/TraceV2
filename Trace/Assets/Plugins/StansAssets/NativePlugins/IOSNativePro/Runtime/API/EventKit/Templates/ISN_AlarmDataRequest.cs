using SA.Foundation.Time;
using UnityEngine;
using System;

namespace SA.iOS.EventKit
{
    /// <summary>
    /// This is object for creating new alarm for Events and Reminders.
    /// </summary>
    [Serializable]
    public class ISN_AlarmDataRequest
    {
#pragma warning disable 414
        [SerializeField] private bool m_HasAlarm;
        [SerializeField] private bool m_isAbsoluteDate;
        [SerializeField] long m_DueDate = -1;
        [SerializeField] long m_TimeStamp = -1;
#pragma warning restore 414
        
        /// <summary>
        /// Create an ISN_AlarmDataRequest object.
        /// </summary>
        public ISN_AlarmDataRequest()
        {
            m_HasAlarm = false;
            m_isAbsoluteDate = false;
        }

        /// <summary>
        /// Create an ISN_AlarmDataRequest object.
        /// </summary>
        /// <param name="dueDate"> Due date of this alarm. </param>
        public ISN_AlarmDataRequest(DateTime dueDate)
        {
            m_HasAlarm = true;
            this.m_DueDate = SA_Unix_Time.ToUnixTime(dueDate);
            m_isAbsoluteDate = true;
        }

        /// <summary>
        /// Create an ISN_AlarmDataRequest object.
        /// </summary>
        /// <param name="timeStamp"> Time stamp before this alarm will activeted. </param>
        public ISN_AlarmDataRequest(long timeStamp)
        {
            m_HasAlarm = true;
            m_isAbsoluteDate = false;
            this.m_TimeStamp = timeStamp;
        }
    }
}
