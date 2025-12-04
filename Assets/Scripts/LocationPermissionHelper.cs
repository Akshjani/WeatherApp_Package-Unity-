using UnityEngine;
using System.Collections;

public static class LocationPermissionHelper
{
    public static IEnumerator RequestLocationPermission()
    {
#if UNITY_ANDROID
        // If already granted
        if (UnityEngine.Android.Permission.HasUserAuthorizedPermission(UnityEngine.Android.Permission.FineLocation))
            yield break;

        // Ask permission
        UnityEngine.Android.Permission.RequestUserPermission(UnityEngine.Android.Permission.FineLocation);

        // Wait until user responds
        while (!UnityEngine.Android.Permission.HasUserAuthorizedPermission(UnityEngine.Android.Permission.FineLocation))
        {
            Debug.Log("Waiting for Android location permission...");
            yield return null;
        }

#elif UNITY_IOS
        // iOS requests location automatically when Input.location.Start() is called.
        yield return null;
#endif
    }
}
