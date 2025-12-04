using System;
using System.Collections;
using UnityEngine;

public class LocationService : MonoBehaviour
{
    private const int MAX_WAIT_TIME = 20;

    public delegate void LocationCallback(float latitude, float longitude, string error);

    public IEnumerator GetCurrentLocation(LocationCallback callback)
    {
        // Check if location services are enabled
        if (!Input.location.isEnabledByUser)
        {
            callback?.Invoke(0, 0, "Location services are not enabled");
            yield break;
        }

        // Start location service
        Input.location.Start(10f, 10f);

        // Wait for initialization
        int maxWait = MAX_WAIT_TIME;
        while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
        { 
            yield return new WaitForSeconds(1);
            maxWait--;
        }

        // Check if timed out
        if (maxWait <= 0)
        {
            callback?.Invoke(0, 0, "Location service initialization timed out");
            yield break;
        }

        // Check for errors
        if (Input.location.status == LocationServiceStatus.Failed)
        {
            callback?.Invoke(0, 0, "Unable to determine device location");
            yield break;
        }

        // Success - get location data
        float latitude = Input.location.lastData.latitude;
        float longitude = Input.location.lastData.longitude;

        callback?.Invoke(latitude, longitude, null);

        // Stop location service
        Input.location.Stop();
    }
}