using SujaySarma.Sdk.RestApi;
using SujaySarma.Web.TravelWeb.Data.AzureMaps;

using System.Text.Json;

namespace SujaySarma.Web.TravelWeb.ApiClients
{
    /// <summary>
    /// Client to interact with Azure Maps API
    /// </summary>
    internal static class AzureMapsApiClient
    {

        /// <summary>
        /// Perform a reverse geo lookup
        /// </summary>
        /// <param name="subscriptionKey">Azure Maps Subscription key</param>
        /// <param name="lat">Latitude to lookup</param>
        /// <param name="lon">Longitude to lookup</param>
        /// <returns>Address if found, else NULL</returns>
        public static ReverseGeoAddressItem? ReverseGeoLookup(string subscriptionKey, double lat, double lon)
        {
            RestApiClient client = new()
            {
                RequestUri = new Uri($"{AZURE_MAPS_BASE_URL}/search/address/reverse/json?api-version=1.0&subscription-key={subscriptionKey}&query={lat:N5},{lon:N5}&language=en-US")
            };

            HttpResponseMessage responseMessage = client.Get();
            if (responseMessage.IsSuccessStatusCode)
            {
                ReverseGeoLookupResult? result = JsonSerializer.Deserialize<ReverseGeoLookupResult>(responseMessage.Content.ReadAsStringAsync().Result);
                if ((result != null) && (result.Summary != null) && (result.Summary["numResults"] > 0))
                {
                    return result.GetAddressItem();
                }
            }

            return null;
        }


        private static readonly string AZURE_MAPS_BASE_URL = "https://atlas.microsoft.com";
    }
}
