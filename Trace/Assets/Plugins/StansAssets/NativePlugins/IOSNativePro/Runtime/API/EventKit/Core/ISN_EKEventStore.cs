using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SA.iOS.Utilities;
using SA.Foundation.Templates;
using System;
using UnityEngine.Assertions;
using SA.iOS.EventKit.Internal;

namespace SA.iOS.EventKit
{
    /// <summary>
    /// Event Kit store that gives access to Event Kit functionality.
    /// </summary>
    public class EKEventStore
    {
        private static EKEventStore m_Instance;

        /// <summary>
        /// Get Instance of Event Kit store
        /// </summary>
        public static EKEventStore Instance
        {
            get
            {
                if(m_Instance == null)
                {
                    m_Instance = new EKEventStore();
                }
                return m_Instance;
            }
        }

        /// <summary>
        /// Request access to EventKit Event.
        /// </summary>
        /// <param name="callback">
        /// This is callback that will be called from EventKit
        /// when user will give acccess or not to iOS calendar functionality.
        /// It shouldn't be null.
        /// </param>
        public void RequestAccessToEvent(Action<SA_Result> callback)
        {
            Assert.IsNotNull(callback);
            ISN_EventKitLib.API.EventKitRequestAccess(callback, ISN_EntityType.Event);
        }

        /// <summary>
        /// Request access to EventKit Reminder.
        /// </summary>
        /// <param name="callback">
        /// This is callback that will be called from EventKit when user will give acccess or not to iOS calendar functionality.
        /// It shouldn't be null.</param>
        public void RequestAccessToReminder(Action<SA_Result> callback)
        {
            Assert.IsNotNull(callback);
            ISN_EventKitLib.API.EventKitRequestAccess(callback, ISN_EntityType.Reminder);
        }


        /// <summary>
        /// Save new event though EventKit.
        /// </summary>
        /// <param name="title">Title of the event.</param>
        /// <param name="startDate">Start date of this event.</param>
        /// <param name="endDate">End date of this event.</param>
        /// <param name="callback">
        /// This is callback that will be called from EventKit when user will give acccess or not to iOS calendar functionality.
        /// It shouldn't be null.</param>
        public void SaveEvent(string title, DateTime startDate, DateTime endDate, Action<ISN_EventKitSaveResult> callback)
        {
            Assert.IsNotNull(callback);
            ISN_EventKitDataRequest request = new ISN_EventKitDataRequest(title, startDate, endDate);
            ISN_EventKitLib.API.SaveEvent(callback, request, null, null);
        }

        /// <summary>
        /// Save new event with alarm and recurrencerule though EventKit.
        /// </summary>
        /// <param name="title"> Title of the event.</param>
        /// <param name="startDate"> Start date of this event.</param>
        /// <param name="endDate"> End date of this event.></param>
        /// <param name="alarm"> Alarm that should be added to this event. </param>
        /// <param name="recurrenceRule"> The recurrence rule that should be added to this event.</param>
        /// <param name="callback">
        /// This is callback that will be called from EventKit when user will give acccess or not to iOS calendar functionality.
        /// It shouldn't be null.</param>
        public void SaveEvent(string title, DateTime startDate, DateTime endDate, ISN_AlarmDataRequest alarm, ISN_RecurrenceRuleRequest recurrenceRule, Action<ISN_EventKitSaveResult> callback)
        {
            Assert.IsNotNull(callback);
            ISN_EventKitDataRequest request = new ISN_EventKitDataRequest(title, startDate, endDate);
            ISN_EventKitLib.API.SaveEvent(callback, request, alarm, recurrenceRule);
        }

        /// <summary>
        /// Remove event though EventKit by it identifier.
        /// </summary>
        /// <param name="identifier"> Identifier of the created event in the EventStore.</param>
        /// <param name="callback">
        /// This is callback that will be called from EventKit when user will give acccess or not to iOS calendar functionality.
        /// It shouldn't be null.</param>
        public void RemoveEvent(string identifier, Action<SA_Result> callback)
        {
            Assert.IsNotNull(callback);
            ISN_EventKitLib.API.RemoveEvent(callback, identifier);
        }


        /// <summary>
        /// Save new reminder though EventKit.
        /// </summary>
        /// <param name="title"> Title of the reminder.</param>
        /// <param name="callback">
        /// This is callback that will be called from EventKit when user will give acccess or not to iOS calendar functionality.
        /// It shouldn't be null.</param>
        public void SaveReminder(string title, Action<ISN_EventKitSaveResult> callback)
        {
            Assert.IsNotNull(callback);
            ISN_EventKitDataRequest request = new ISN_EventKitDataRequest(title);
            ISN_EventKitLib.API.SaveReminder(callback, request, null, null);
        }

        /// <summary>
        /// Save new reminder with added alarm and recurrent rule though EventKit.
        /// </summary>
        /// <param name="title"> Title of the reminder.</param>
        /// <param name="startDate"> Start date of this reminder.</param>
        /// <param name="endDate"> End date of this reminder.></param>
        /// <param name="alarm"> Alarm that should be added to this reminder. </param>
        /// <param name="recurrenceRule"> The recurrence rule that should be added to this reminder.</param>
        /// <param name="callback">
        /// This is callback that will be called from EventKit when user will give acccess or not to iOS calendar functionality.
        /// It shouldn't be null.</param>
        public void SaveReminder(string title, DateTime startDate, DateTime endDate, ISN_AlarmDataRequest alarm, ISN_RecurrenceRuleRequest recurrenceRule, Action<ISN_EventKitSaveResult> callback)
        {
            Assert.IsNotNull(callback);
            ISN_EventKitDataRequest request = new ISN_EventKitDataRequest(title, startDate, endDate);
            ISN_EventKitLib.API.SaveReminder(callback, request, alarm, recurrenceRule);
        }


        /// <summary>
        /// Remove reminder though EventKit by it identifier.
        /// </summary>
        /// <param name="identifier"> Identifier of the created reminder in the Calendar.</param>
        /// <param name="callback">
        /// This is callback that will be called from EventKit when user will give acccess or not to iOS calendar functionality.
        /// It shouldn't be null.</param>
        public void RemoveReminder(string identifier, Action<SA_Result> callback)
        {
            Assert.IsNotNull(callback);
            ISN_EventKitLib.API.RemoveReminder(callback, identifier);
        }

    }
}
