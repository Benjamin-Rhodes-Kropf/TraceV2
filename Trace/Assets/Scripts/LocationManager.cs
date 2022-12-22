using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocationManager : MonoBehaviour
{
    //Dont Destroy
    private static LocationManager instance;
    
    // The latitude and longitude of the user's device
    private double latitude;
    public double MyLatitude
    {
        get { return latitude; }
        private set { latitude = value; }
    }
    
    
    private double longitude;
    public double MyLongitude
    {
        get { return latitude; }
        private set { latitude = value; }
    }
    
    private void Awake()
    {
        if (instance != null)
        {Destroy(gameObject);}
        instance = this;
        DontDestroyOnLoad(this.gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
            Debug.Log("LocationManager: attempting to get IOS user location");
            
            // Check if location service is enabled by the user
            if (!Input.location.isEnabledByUser)
            {
                Debug.Log("LocationManager: Location services are not enabled by the user.");
                return;
            }

            // Start service before querying location
            Input.location.Start();

            // Wait until service initializes
            int maxWait = 20;
            while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
            {
                maxWait--;
                System.Threading.Thread.Sleep(100);
            }

            // Service didn't initialize in 20 seconds
            if (maxWait < 1)
            {
                Debug.Log("LocationManager: Timed out");
                return;
            }

            // Connection has failed
            if (Input.location.status == LocationServiceStatus.Failed)
            {
                Debug.Log("LocationManager: Unable to determine device location.");
                return;
            }
            else
            {
                // Access granted and location value could be retrieved
                MyLatitude = Input.location.lastData.latitude;
                MyLongitude = Input.location.lastData.longitude;
                Debug.Log("LocationManager: Location: " + MyLatitude + ", " + MyLongitude);
            }

            // Stop service if there is no need to query location updates continuously
            Input.location.Stop();
        }
        else if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.OSXEditor)
        {
            Debug.Log("LocationManager: attempting to get UNITY_EDITOR user location");
            if ((int)Input.location.status != (int)LocationServiceStatus.Running)
            {
                Debug.Log("LocationManager: Location services  is not running set lat and long manually");
            }
        }
        else
        {
            Debug.Log("LocationManager: Location services are not supported on this platform.");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
