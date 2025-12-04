using UnityEngine;
using System.Collections;

namespace CleverTap.ToastPackage
{
    public class ToastManager : MonoBehaviour
    {
        private static ToastManager _instance;
        private Coroutine _currentToastCoroutine;

        public static ToastManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    GameObject go = new GameObject("ToastManager");
                    _instance = go.AddComponent<ToastManager>();
                    DontDestroyOnLoad(go);
                    Debug.Log("[ToastManager] Instance created");
                }
                return _instance;
            }
        }

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Debug.Log("[ToastManager] Duplicate instance destroyed");
                Destroy(gameObject);
                return;
            }
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }

        /// <summary>
        /// Shows a toast message on Android or snackbar on iOS
        /// </summary>
        /// <param name="message">The message to display</param>
        /// <param name="duration">Duration: 0 = short (~2s), 1 = long (~3.5s)</param>
        public void ShowToast(string message, int duration = 0)
        {
            if (string.IsNullOrEmpty(message))
            {
                Debug.LogWarning("[ToastManager] Empty message provided");
                return;
            }

            Debug.Log($"[ToastManager] Showing toast: {message} (duration: {duration})");

#if UNITY_ANDROID && !UNITY_EDITOR
            // Android: Show native toast
            AndroidToastBridge.ShowToast(message, duration);
#elif UNITY_IOS && !UNITY_EDITOR
            // iOS: Show native snackbar
            IOSSnackbarBridge.ShowSnackbar(message, duration);
#else
            // Editor: Use coroutine for visual feedback in console
            if (_currentToastCoroutine != null)
            {
                StopCoroutine(_currentToastCoroutine);
            }
            _currentToastCoroutine = StartCoroutine(SimulateToastInEditor(message, duration));
#endif
        }

        /// <summary>
        /// Shows a short toast (~2 seconds)
        /// </summary>
        public void ShowShortToast(string message)
        {
            ShowToast(message, 0);
        }

        /// <summary>
        /// Shows a long toast (~3.5 seconds)
        /// </summary>
        public void ShowLongToast(string message)
        {
            ShowToast(message, 1);
        }

        /// <summary>
        /// Simulates toast behavior in Unity Editor for testing
        /// </summary>
        private IEnumerator SimulateToastInEditor(string message, int duration)
        {
            float displayTime = duration == 1 ? 3.5f : 2.0f;
            Debug.Log($"  TOAST: {message}");
            yield return new WaitForSeconds(displayTime);
            Debug.Log($"[ToastManager] Toast dismissed after {displayTime}s");
        }

        private void OnDestroy()
        {
            if (_instance == this)
            {
                _instance = null;
                Debug.Log("[ToastManager] Instance destroyed");
            }
        }
    }
}