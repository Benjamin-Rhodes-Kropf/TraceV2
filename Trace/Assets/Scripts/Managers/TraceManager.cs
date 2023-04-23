using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Notifications.iOS;
using UnityEngine;

public class TraceManager : MonoBehaviour
{
    // =========================NOTE============================ //
    // Required package "com.unity.mobile.notifications": "2.1.1",
    
    [SerializeField] private OnlineMapsLocationService onlineMapsLocationService;
    [SerializeField] private Vector2 userLocation;
    public List<TraceObject> traceObjects;
    public List<TraceObject> traceObjectsByDistanceToUser;
    
    
    [Header("Variables")] 
    [SerializeField] private float startingPointLatitude;
    [SerializeField] private float startingPointLongitude;
    
    [Header("Maximum Distance in meters")]
    [SerializeField] private double maxDist;
    
    private bool _areTracesInitialized;
    private double _distance;
    private float _previousLatitude, _previousLongitude;
    
    
    // Start is called before the first frame update
    void Start()
    {
        traceObjectsByDistanceToUser = new List<TraceObject>();

        _areTracesInitialized = false;

        // Allows app to run in background
        Application.runInBackground = true;
    }
    
    private static double ApproximateDistanceBetweenTwoLatLongsInM(double lat1, double lon1, double lat2, double lon2)
    {
        var ldRadians = lat1 / 57.3 * 0.017453292519943295769236907684886;
        var ldCosR = Math.Cos(ldRadians);
        var x = 69.1 * (lat2 - lat1);
        var y = 69.1 * (lon2 - lon1) * ldCosR;

        return Math.Sqrt(x * x + y * y) * 1.609344 * 1000; /* Converts mi to km to m. */
    }
    private double CalculateTheDistanceBetweenCoordinatesAndCurrentCoordinates(float lat1, float lon1, float lat2, float lon2, float radiusOfTraceInMeters)
    {
        double distance;
        var R = 6378.137; // Radius of earth in KM
        var dLat = lat2 * Mathf.PI / 180 - lat1 * Mathf.PI / 180;
        var dLon = lon2 * Mathf.PI / 180 - lon1 * Mathf.PI / 180;
        float a = Mathf.Sin(dLat / 2) * Mathf.Sin(dLat / 2) +
                  Mathf.Cos(lat1 * Mathf.PI / 180) * Mathf.Cos(lat2 * Mathf.PI / 180) *
                  Mathf.Sin(dLon / 2) * Mathf.Sin(dLon / 2);
        var c = 2 * Mathf.Atan2(Mathf.Sqrt(a), Mathf.Sqrt(1 - a));
        distance = R * c;
        distance = distance * 1000f; // meters
        distance -= radiusOfTraceInMeters; //account for trace radius
        return distance;
    }

    public IOrderedEnumerable<TraceObject> OrderTracesByDistanceToUser()
    {
        foreach (var traceObject in traceObjects)
        {
            traceObject.distanceToUser = CalculateTheDistanceBetweenCoordinatesAndCurrentCoordinates(
                userLocation.x,userLocation.y,traceObject.lat, traceObject.lng, traceObject.radius);
        }
        var traceObjectsInOrderOfDistance = traceObjects.OrderBy(f => f.distanceToUser);
        return traceObjectsInOrderOfDistance;
    }
    
    private List<TraceObject> ApplyDistanceFilterTraces(float userLat, float userLon)
    {
        var filtered = new List<(TraceObject, double)>();
        foreach (var trace in traceObjects)
        {
            var distance = ApproximateDistanceBetweenTwoLatLongsInM(userLat, userLon, trace.lat,
                trace.lng);
            filtered.Add((trace, distance));
        }

        filtered.Sort((i1, i2) => i1.Item2.CompareTo(i2.Item2));
        return filtered.Select(i => i.Item1).ToList();
    }
    
    private void UpdateNotificationsForNext10Traces()
    {
        if (traceObjects.Count < 1)
        {
            Debug.Log("UpdateNotificationsForNextTraces: No Traces Available!");
            return;
        }

        traceObjects = ApplyDistanceFilterTraces(_previousLatitude, _previousLongitude);

        for (var i = 0; i < traceObjects.Count && i < 10; i++)
        {
            var trace = traceObjects[i];
            ScheduleNotificationOnEnterInARadius(trace.lat, trace.lng,trace.radius, trace.text);
            //ScheduleNotificationOnExitFromARadius(trace.lat, trace.lng, trace.text);
        }
    }
    
    private static void ScheduleNotificationOnEnterInARadius(float latitude, float longitude, float radius, string message)
    {
        var enterLocationTrigger = new iOSNotificationLocationTrigger
        {
            Center = new Vector2(latitude, longitude),
            Radius = radius,
            NotifyOnEntry = true,
            NotifyOnExit = false
        };
        Debug.Log("Push Notification is set for a radius of " + enterLocationTrigger.Radius + "Meters"
                  + " When user enters in " + "Latitude = " + latitude + "===" + "Longitude = " + longitude);

        var entryBasedNotification = new iOSNotification
        {
            Title = "Exited",
            Subtitle = "You Have Entered in the radius of " + enterLocationTrigger.Center + " Meters",
            Body = message == "" ? "Radius latitude was > " + latitude + " and longitude was > " + longitude : message,
            ShowInForeground = true,
            ForegroundPresentationOption = PresentationOption.Alert | PresentationOption.Sound,
            Trigger = enterLocationTrigger
        };
        
        iOSNotificationCenter.ScheduleNotification(entryBasedNotification);
        // Schedule notification for entry base
        Debug.Log("Started a location notification at " + enterLocationTrigger.Center);
    }

    private void UpdateNotificationForNextTrace()
    {
        if (traceObjects.Count < 1)
        {
            Debug.Log("UpdateNotificationForNextTrace: No Traces Available!");
            return;
        }

        var trace = traceObjects[0];
        ScheduleNotificationOnEnterInARadius(trace.lat, trace.lng, trace.radius, trace.text);
        //ScheduleNotificationOnExitFromARadius(trace.lat, trace.lng, trace.text);
    }

    public void StopLocationServices()
    {
        // If required then we can stop the location service by this
        Input.location.Stop();
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 previousUserLocation = userLocation;
        if (onlineMapsLocationService.IsLocationServiceRunning())
        {
            userLocation = onlineMapsLocationService.position;
        }
        else
        {
            userLocation = onlineMapsLocationService.emulatorPosition;
        }
        if (previousUserLocation != userLocation)
        {
            traceObjectsByDistanceToUser = OrderTracesByDistanceToUser().ToList();
        }
        
        var currentLatitude = Input.location.lastData.latitude;
        var currentLongitude = Input.location.lastData.longitude;

        // Showing current updated coordinates

        //distanceDetails.text = "Current Distance from Start Point is = " + CalculateDistance();
        _distance = ApproximateDistanceBetweenTwoLatLongsInM(_previousLatitude, _previousLongitude, currentLatitude, currentLongitude);

        // Detecting the Significant Location Change
        if (_distance > maxDist)
        {
            // Remove All Pending Notifications
            iOSNotificationCenter.RemoveAllScheduledNotifications();

            // Set current player's location
            _previousLatitude = currentLatitude;
            _previousLongitude = currentLongitude;

            // Add Notifications for the Next 10 Distance Filtered Traces
            UpdateNotificationsForNext10Traces();
        }
    }
}

[Serializable]
public class TraceObject
{
    public float lat;
    public float lng;
    public float radius;
    public double distanceToUser;
    public string text;
    public double startTimeStamp;
    public double endTimeStamp;
    
    public TraceObject(float longitude, float latitude, float radius, double startTimeStamp, double endTimeStamp)
    {
        lng = longitude;
        lat = latitude;
        this.radius = radius;
        this.startTimeStamp = startTimeStamp;
        this.endTimeStamp = endTimeStamp;
    }
}