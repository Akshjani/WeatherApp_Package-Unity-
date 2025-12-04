using UnityEngine;
using UnityEngine.UI;
using CleverTap.ToastPackage;
using TMPro;
using System.Collections;

public class WeatherApp : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI temperatureText;
    [SerializeField] private TextMeshProUGUI locationText;
    [SerializeField] private TextMeshProUGUI statusText;
    [SerializeField] private Button fetchWeatherButton;
    [SerializeField] private Button showToastButton;

    private LocationService _locationService;
    private WeatherAPIClient _weatherAPIClient;
    private float _currentLatitude;
    private float _currentLongitude;
    private float _currentTemperature;

    private void Awake()
    {
        _locationService = gameObject.AddComponent<LocationService>();
        _weatherAPIClient = gameObject.AddComponent<WeatherAPIClient>();


        if (fetchWeatherButton != null)
            fetchWeatherButton.onClick.AddListener(OnFetchWeatherClicked);

        if (showToastButton != null)
            showToastButton.onClick.AddListener(OnShowToastClicked);
    }

    private void Start()
    {
        UpdateUI("Ready", "", "");
        FetchWeather();
    }
    public void FetchWeather()
    {
        StartCoroutine(RequestPermissionAndFetch());
    }

    private IEnumerator RequestPermissionAndFetch()
    {
        UpdateUI("Requesting location permission...", "", "");

        yield return LocationPermissionHelper.RequestLocationPermission();

        UpdateUI("Getting location...", "", "");

        StartCoroutine(_locationService.GetCurrentLocation(OnLocationReceived));
    }

    private void OnLocationReceived(float latitude, float longitude, string error)
    {
        if (!string.IsNullOrEmpty(error))
        {
            UpdateUI($"Location Error: {error}", "", "");
            ToastManager.Instance.ShowLongToast($"Location Error: {error}");
            return;
        }

        _currentLatitude = latitude;
        _currentLongitude = longitude;

        UpdateUI("Fetching weather data...", $"Lat: {latitude:F2}, Lon: {longitude:F2}", "");

        _weatherAPIClient.GetWeatherData(latitude, longitude, OnWeatherDataReceived);
    }

    private void OnWeatherDataReceived(WeatherResponse data, string error)
    {
        if (!string.IsNullOrEmpty(error))
        {
            UpdateUI($"Weather Error: {error}", "", "");
            ToastManager.Instance.ShowLongToast($"Weather Error: {error}");
            return;
        }

        if (data != null && data.daily != null && data.daily.temperature_2m_max != null && data.daily.temperature_2m_max.Length > 0)
        {
            _currentTemperature = data.daily.temperature_2m_max[0];
            string location = $"Lat: {data.latitude:F2}, Lon: {data.longitude:F2}\n{data.timezone}";
            string temperature = $"{_currentTemperature:F1}°C";

            UpdateUI("Weather data loaded successfully!", location, temperature);
            ToastManager.Instance.ShowLongToast($"Current Temperature: {_currentTemperature:F1}°C");
        }
        else
        {
            UpdateUI("Invalid weather data received", "", "");
            ToastManager.Instance.ShowLongToast("Invalid weather data received");
        }
    }

    private void UpdateUI(string status, string location, string temperature)
    {
        if (statusText != null) statusText.text = status;
        if (locationText != null) locationText.text = location;
        if (temperatureText != null) temperatureText.text = temperature;
    }

    private void OnFetchWeatherClicked()
    {
        FetchWeather();
    }

    private void OnShowToastClicked()
    {
        if (_currentTemperature > 0)
        {
            string message = $"Temperature: {_currentTemperature:F1}°C at ({_currentLatitude:F2}, {_currentLongitude:F2})";
            ToastManager.Instance.ShowLongToast(message);
        }
        else
        {
            ToastManager.Instance.ShowShortToast("No weather data available. Please fetch weather first.");
        }
    }

    //private void OnDestroy()
    //{
    //    if (fetchWeatherButton != null)
    //        fetchWeatherButton.onClick.RemoveListener(OnFetchWeatherClicked);

    //    if (showToastButton != null)
    //        showToastButton.onClick.RemoveListener(OnShowToastClicked);
    //}
}