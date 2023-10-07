

using System;
using System.Windows;
using System.Net.Http;
using Microsoft.VisualStudio.Text.Adornments;
using System.Threading;
using System.Diagnostics;

namespace UniversalToolkit
{
    public class weatherEventArgs : EventArgs
    {
        public readonly string summary;

        public weatherEventArgs(string summary)
        {
            this.summary = summary;
        }
    }

    public class Weather
    {
        public Weather()
        {
        }

        

        private bool isReceived = false;
        private string Result { get; set; } = "";

        public string GetWeatherHistory(double lat, double longit, DateTime when)
        {
            try
            {
                GetVCWeatherHistoryAsync(lat, longit, when);
                
                return(Result);
            }
            catch (Exception ex)
            {
                return (ex.Message);
            }

            
        }

        public async void GetVCWeatherHistoryAsync(double latitude,double longitude,DateTime when)
        {
            try
            {
                isReceived = false;
                Result = "";
                string body = "";
                string result = "VisualCrossing:- ";
                string key = APIKeys.VisualCrossingsLicenseKey;
                string dateString = $"{when.Year}-{when.Month:00}-{when.Day:00}T{when.Hour:00}:{when.Minute:00}:{when.Second:00}";
                string locationString = $"{latitude},{longitude}";
                string requestString = $"https://weather.visualcrossing.com/VisualCrossingWebServices/rest/services/timeline/" +
                    $"{locationString}/{dateString}?unitGroup=uk&key={key}" +
                    $"&elements=datetime,temp,cloudcover,windspeed" +
                    $"&nonulls&noheaders&include=current&contentType=csv";
                Debug.WriteLine($"request:- {requestString}");
                var request=new HttpRequestMessage(HttpMethod.Get, requestString);
                var client = new HttpClient();
                
                using(HttpResponseMessage response=client.GetAsync(requestString).GetAwaiter().GetResult())
                {
                    using(HttpContent content=response.Content)
                    {
                        body=content.ReadAsStringAsync().GetAwaiter().GetResult();
                    }
                }

                /*var response = await client.SendAsync(request);
                Debug.WriteLine("sent");
                response.EnsureSuccessStatusCode();// throw an exception if it didnt work
                Debug.WriteLine("awaiting response");
                var body = await response.Content.ReadAsStringAsync();*/
                Debug.WriteLine($"Received {body}");
                string[] lines = body.Split("\n");
                if (lines.Length > 1)
                {
                    string[] elements = lines[1].Split(",");
                    result += $"t={elements[1]??"-"}C, cloud={elements[3]??"?"}%, wind={elements[2]??"?"}mph, at {elements[0]??when.ToString()}";
                }
                Result = result;
                isReceived = true;
               // OnWeatherReceived(new weatherEventArgs(Result));
                

            }catch(HttpRequestException ex)
            {
                Debug.WriteLine($"Exception {ex.Message}");
                Result = "No weather data available";
                isReceived = true;
                
            }
        }


       
    }
}