using SA.Foundation.Templates;
using UnityEngine;

namespace SA.iOS.EventKit.Internal
{
    internal interface ISN_EventKitAPI
    {
        void EventKitRequestAccess(System.Action<SA_Result> callback, ISN_EntityType entityType);
        void SaveEvent(System.Action<ISN_EventKitSaveResult> callback, ISN_EventKitDataRequest eventData, ISN_AlarmDataRequest alarmData, ISN_RecurrenceRuleRequest recurrenceRule);
        void RemoveEvent(System.Action<SA_Result> callback, string identifier);
        void SaveReminder(System.Action<ISN_EventKitSaveResult> callback, ISN_EventKitDataRequest reminderData, ISN_AlarmDataRequest alarmData, ISN_RecurrenceRuleRequest recurrenceRule);
        void RemoveReminder(System.Action<SA_Result> callback, string identifier);
    }
}
