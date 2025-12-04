#if UNITY_IOS
using System.Runtime.InteropServices;

namespace CleverTap.ToastPackage
{
    public static class IOSSnackbarBridge
    {
        [DllImport("__Internal")]
        private static extern void _ShowSnackbar(string message, int duration);

        public static void ShowSnackbar(string message, int duration)
        {
            _ShowSnackbar(message, duration);
        }
    }
}
#endif