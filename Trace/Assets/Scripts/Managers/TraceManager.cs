using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TraceManager : MonoBehaviour
{
    [SerializeField] private OnlineMapsLocationService onlineMapsLocationService;
    [SerializeField] private Vector2 userLocation;
    public List<TraceObject> traceObjects;
    public List<TraceObject> traceObjectsByDistanceToUser;
    
    // Start is called before the first frame update
    void Start()
    {
        traceObjectsByDistanceToUser = new List<TraceObject>();
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
    }
}

[Serializable]
public class TraceObject
{
    public float lat;
    public float lng;
    public float radius;
    public double distanceToUser;
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