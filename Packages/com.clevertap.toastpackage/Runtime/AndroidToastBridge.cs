#if UNITY_ANDROID
using UnityEngine;

namespace CleverTap.ToastPackage
{
    public static class AndroidToastBridge
    {
        public static void ShowToast(string message, int duration)
        {
            try
            {
                // Get Unity Player and current activity freshly each time
                AndroidJavaClass unityPlayerClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
                AndroidJavaObject currentActivity = unityPlayerClass.GetStatic<AndroidJavaObject>("currentActivity");

                if (currentActivity == null)
                {
                    Debug.LogError("[AndroidToastBridge] Current activity is null");
                    return;
                }

                // Run on UI thread
                currentActivity.Call("runOnUiThread", new AndroidJavaRunnable(() =>
                {
                    try
                    {
                        // Create new Toast each time
                        AndroidJavaClass toastClass = new AndroidJavaClass("android.widget.Toast");
                        AndroidJavaObject context = currentActivity;

                        // makeText parameters: Context, CharSequence, duration
                        AndroidJavaObject toastObject = toastClass.CallStatic<AndroidJavaObject>(
                            "makeText",
                            context,
                            message,
                            duration  // 0 = Toast.LENGTH_SHORT, 1 = Toast.LENGTH_LONG
                        );

                        if (toastObject != null)
                        {
                            toastObject.Call("show");
                            Debug.Log($"[AndroidToastBridge] Toast shown: {message}");
                        }
                        else
                        {
                            Debug.LogError("[AndroidToastBridge] Toast object is null");
                        }
                    }
                    catch (System.Exception e)
                    {
                        Debug.LogError($"[AndroidToastBridge] Error in UI thread: {e.Message}\n{e.StackTrace}");
                    }
                }));
            }
            catch (System.Exception e)
            {
                Debug.LogError($"[AndroidToastBridge] Error showing toast: {e.Message}\n{e.StackTrace}");
            }
        }
    }
}
#endif