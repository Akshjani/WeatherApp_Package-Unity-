using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

[Serializable]
public class WeatherResponse
{
    public float latitude;
    public float longitude;
    public string timezone;
    public DailyUnits daily_units;
    public DailyData daily;
}

[Serializable]
public class DailyUnits
{
    public string time;
    public string temperature_2m_max;
}

[Serializable]
public class DailyData
{
    public string[] time;
    public float[] temperature_2m_max;
}

public class WeatherAPIClient : MonoBehaviour
{
    private const string API_BASE_URL = "https://api.open-meteo.com/v1/forecast";

    public delegate void WeatherDataCallback(WeatherResponse data, string error);

    public void GetWeatherData(float latitude, float longitude, WeatherDataCallback callback)
    {
        StartCoroutine(FetchWeatherData(latitude, longitude, callback));
    }

    private IEnumerator FetchWeatherData(float latitude, float longitude, WeatherDataCallback callback)
    {
        string url = $"{API_BASE_URL}?latitude={latitude:F2}&longitude={longitude:F2}&timezone=auto&daily=temperature_2m_max";

        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                try
                {
                    //WeatherResponse weatherData = JsonUtility.FromJson(request.downloadHandler.text);
                    WeatherResponse response = JsonUtility.FromJson<WeatherResponse>(request.downloadHandler.text);

                    callback?.Invoke(response, null);
                }
                catch (Exception e)
                {
                    callback?.Invoke(null, $"JSON Parse Error: {e.Message}");
                }
            }
            else
            {
                callback?.Invoke(null, $"Network Error: {request.error}");
            }
        }
    }
}