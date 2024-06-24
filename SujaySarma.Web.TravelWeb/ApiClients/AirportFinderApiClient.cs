using SujaySarma.Sdk.RestApi;
using SujaySarma.Web.TravelWeb.Data;
using SujaySarma.Web.TravelWeb.Data.Airports;

using System.Text.Json;
using System.Text.Json.Serialization;

namespace SujaySarma.Web.TravelWeb.ApiClients
{
    internal static class AirportFinderApiClient
    {

        /// <summary>
        /// Get the nearest airports to a location
        /// </summary>
        /// <param name="lat">Latitude</param>
        /// <param name="lon">Longitude</param>
        /// <param name="max">Maximum results to return</param>
        /// <returns>List of airports found or NULL</returns>
        public static List<Airport>? GetNearest(double lat, double lon, int max = 5)
        {
            RestApiClient client = new()
            {
                RequestUri = new Uri($"{BASE_URI}/api/airports/nearest?latlon={lat:N5},{lon:N5}&iataOnly=true&max={max}")
            };

            HttpResponseMessage responseMessage = client.Get();
            if (responseMessage.IsSuccessStatusCode)
            {
                string s = responseMessage.Content.ReadAsStringAsync().Result;
                AirportApiResult? results = JsonSerializer.Deserialize<AirportApiResult>(s);

                if ((results != null) && (results.Count > 0) && (results.AirportsByDistance != null))
                {
                    List<Airport> airports = new();
                    foreach(AirportByDistance item in results.AirportsByDistance)
                    {
                        if (item.Airport != null)
                        {
                            item.Airport.DistanceFromLocation = item.DistanceInKm;
                            airports.Add(item.Airport);
                        }
                    }

                    return airports;
                }
            }

            return null;
        }


        private static readonly string BASE_URI = "https://airportfinder.sujaytravelapps.com";
    }


    internal class AirportApiResult
    {
        
        [JsonPropertyName("count")]
        public int Count { get; set; }


        [JsonPropertyName("results")]
        public List<AirportByDistance>? AirportsByDistance { get; set; }
    }


    internal class AirportByDistance
    {
        [JsonPropertyName("dist_km")]
        public double DistanceInKm { get; set; }

        [JsonPropertyName("airport")]
        public Airport? Airport { get; set; }
    }
}
