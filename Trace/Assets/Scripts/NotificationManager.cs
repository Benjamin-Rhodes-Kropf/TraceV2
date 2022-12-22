using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NotificationManager : MonoBehaviour
{
    // Schedule a local notification with the specified parameters
    public void ScheduleNotification(string title, string body, int seconds)
    {
        UnityEngine.iOS.LocalNotification notification = new UnityEngine.iOS.LocalNotification();
        notification.fireDate = System.DateTime.Now.AddSeconds(seconds);
        notification.alertBody = body;
        notification.alertAction = title;
        notification.hasAction = true;
        notification.applicationIconBadgeNumber = 1;
        UnityEngine.iOS.NotificationServices.ScheduleLocalNotification(notification);
    }

    // Present the most recently scheduled notification
    public void PresentNotification()
    {
        UnityEngine.iOS.LocalNotification notification = UnityEngine.iOS.NotificationServices.localNotifications[0];
        UnityEngine.iOS.NotificationServices.PresentLocalNotificationNow(notification);
    }

    // Cancel all scheduled notifications
    public void CancelAllNotifications()
    {
        UnityEngine.iOS.NotificationServices.CancelAllLocalNotifications();
    }
    
    //get upcoming notifications
    public void GetUpcomingNotifications()
    {
        UnityEngine.iOS.LocalNotification[] notifications = UnityEngine.iOS.NotificationServices.scheduledLocalNotifications;
        Debug.Log("Upcoming notifications:");
        foreach (UnityEngine.iOS.LocalNotification notification in notifications)
        {
            Debug.Log("Title: " + notification.alertAction);
            Debug.Log("Body: " + notification.alertBody);
            Debug.Log("Fire date: " + notification.fireDate);
        }
    }
}

//how to use:
/*
NotificationManager manager = new NotificationManager();

// Schedule a notification to be presented in 10 seconds
manager.ScheduleNotification("Notification", "This is a notification", 10);

// Present the notification immediately
manager.PresentNotification();

// Cancel all notifications
manager.CancelAllNotifications();
 */