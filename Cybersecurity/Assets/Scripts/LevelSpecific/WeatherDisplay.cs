using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WeatherDisplay : MonoBehaviour
{
    public enum WeatherType
    {
        Sunny,
        Cloudy
    }

    [SerializeField]
    private List<TextMeshProUGUI> m_Labels;

    private Coroutine m_GetCurrentLocationRoutine;
    private Coroutine m_GetCurrentWeatherRoutine;

    private void Start()
    {
        //Query current location
        GetCurrentLocation();
    }

    private void OnLocationReceived(float lat, float lon)
    {
        //Debug.Log("Lat: " + lat + " Lon: " + lon);

        //Query current weather from our location
        GetCurrentWeather(lat, lon);
    }

    private void OnWeatherReceived(float tempCelcius, WeatherType weatherType)
    {
        foreach(TextMeshProUGUI label in m_Labels)
        {
            if (weatherType == WeatherType.Sunny)
            {
                label.text = "\uf185 " + tempCelcius + "°C";
            }
            
            if (weatherType == WeatherType.Cloudy)
            {
                label.text = "\uf0c2 " + tempCelcius + "°C";
                
            }
        }
    }


    //Current location routine
    private void GetCurrentLocation()
    {
        //Don't spam this! (max 150 per minute)
        if (m_GetCurrentLocationRoutine != null)
            StopCoroutine(m_GetCurrentLocationRoutine);

        m_GetCurrentLocationRoutine = StartCoroutine(GetCurrentLocationRoutine());
    }

    private IEnumerator GetCurrentLocationRoutine()
    {
        bool success = true;
        float lat = 0;
        float lon = 0;

        //Get the data
        WWW www = new WWW("http://ip-api.com/json");
        yield return www;

        if (!string.IsNullOrEmpty(www.error))
        {
            Debug.LogError("IP API encountered an error: " + www.error.ToString());
            success = false;
        }

        else if (!string.IsNullOrEmpty(www.text))
        {
            //http://answers.unity3d.com/questions/844423/wwwtext-not-reading-utf-8-text.html
            string fileText = www.text;

            //Deserialize the data
            JSONNode jsonRoot = JSON.Parse(fileText);
            lat = jsonRoot["lat"].AsFloat;
            lon = jsonRoot["lon"].AsFloat;
            success = true;
        }

        //Cleanup
        www.Dispose();

        if (success) { OnLocationReceived(lat, lon); }

        m_GetCurrentLocationRoutine = null;

        yield return null;
    }


    //Current weather routine
    private void GetCurrentWeather(float lat, float lon)
    {
        //Don't spam this! (max 60 per minute)
        if (m_GetCurrentWeatherRoutine != null)
            StopCoroutine(m_GetCurrentWeatherRoutine);

        m_GetCurrentWeatherRoutine = StartCoroutine(GetCurrentWeatherRoutine(lat, lon));
    }

    private IEnumerator GetCurrentWeatherRoutine(float lat, float lon)
    {
        bool success = true;

        float tempCelcius = 0;
        WeatherType weatherType = WeatherType.Sunny;

        //Get the data
        WWW www = new WWW("api.openweathermap.org/data/2.5/weather?lat=" + lat + "&lon=" + lon + "&appid=2260dfaf70bfd46c9cf64de48e26ff28");
        yield return www;

        if (!string.IsNullOrEmpty(www.error))
        {
            Debug.LogError("Weather API encountered an error: " + www.error.ToString());
            success = false;
        }

        else if (!string.IsNullOrEmpty(www.text))
        {
            //http://answers.unity3d.com/questions/844423/wwwtext-not-reading-utf-8-text.html
            string fileText = www.text;

            //Deserialize the data
            JSONNode jsonRoot = JSON.Parse(fileText);
            int cod = jsonRoot["cod"].AsInt;

            //Blocked
            if (cod != 429)
            {
                float tempKelvin = jsonRoot["main"]["temp"].AsFloat;

                //Weather type
                JSONArray weather = jsonRoot["weather"].AsArray;

                int timesSunny = 0;
                int timesCloudy = 0;

                for (int i = 0; i < weather.Count; ++i)
                {
                    string weatherIcon = weather[i]["icon"].ToString();

                    int weatherIconID = -1;
                    bool iconSuccess = int.TryParse(weatherIcon.Substring(1, 2), out weatherIconID);

                    if (iconSuccess)
                    {
                        //Debug.Log("weather icon " + weatherIconID);

                        if (weatherIconID > 4)
                        {
                            timesCloudy++;
                        }
                        else
                        {
                            timesSunny++;
                        }
                    }
                }

                //Debug.Log("Times sunny " + timesSunny);
                //Debug.Log("Times cloudy " + timesCloudy);

                if (timesCloudy > timesSunny)
                {
                    weatherType = WeatherType.Cloudy;
                }

                //Convert Kelvin to celcius
                tempCelcius = tempKelvin - 273.15f;
                tempCelcius = Mathf.RoundToInt(tempCelcius);

                success = true;
            }
        }

        //Cleanup
        www.Dispose();

        if (success) { OnWeatherReceived(tempCelcius, weatherType); }

        m_GetCurrentLocationRoutine = null;

        yield return null;
    }
}
