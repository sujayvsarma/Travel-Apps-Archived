using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Clients.ActiveDirectory;

using SujaySarma.Sdk.AspNetCore.Mvc;
using SujaySarma.Sdk.RestApi;

using System;
using System.Net.Http;
using System.Text.Json;

using TravelIdeasPortalWeb.Objects.AzureMaps;

namespace TravelIdeasPortalWeb.ServiceClients
{
    /// <summary>
    /// Api client to communicate with Azure Maps
    /// </summary>
    internal static class AzureMapsApiClient
    {

        /// <summary>
        /// Get what Azure Maps knows about the provided coordinates
        /// </summary>
        /// <param name="lat">Latitude</param>
        /// <param name="lon">Longitude</param>
        /// <returns>Information found, NULL if there was nothing useful about the location</returns>
        public static ReverseGeoResult GetLocationInfo(double lat, double lon)
        {
            RestApiClient client = new RestApiClient(new Uri($"{AZURE_MAPS_BASE_URL}/search/address/reverse/json?api-version=1.0&subscription-key={MAPS_AUTHZ_PRIMARYKEY}&query={lat.ToString("N5")},{lon.ToString("N5")}&language=en-US"));
            //client.RequestHeaders.Add("x-ms-client-id", AZURE_SUBSCRIPTION_ID);
            //client.RequestHeaders.Add("Authorization", "Bearer " + GetBearerToken());

            HttpResponseMessage responseMessage = client.Get();
            if (responseMessage.IsSuccessStatusCode)
            {
                ReverseGeoResult result = JsonSerializer.Deserialize<ReverseGeoResult>(responseMessage.Content.ReadAsStringAsync().Result);
                if (result.Summary["numResults"] > 0)
                {
                    return result;
                }
            }

            return null;
        }

        /// <summary>
        /// Get the bearer token for Azure Maps
        /// </summary>
        /// <returns>Bearer token for Azure maps (NULL if there was a problem!)</returns>
        public static string GetBearerToken()
        {
            AuthenticationContext authenticationContext = new AuthenticationContext("https://login.microsoftonline.com/sujaylive.onmicrosoft.com", false);
            ClientCredential clientCred = new ClientCredential(MAPS_CLIENT_ID, MAPS_AUTHZ_PRIMARYKEY);
            AuthenticationResult authenticationResult = authenticationContext.AcquireTokenAsync("https://atlas.microsoft.com/ ", clientCred).Result;

            return authenticationResult.AccessToken;
        }


        static AzureMapsApiClient()
        {
            IConfigurationSection mapsConfig = AppSettingsJson.Configuration.GetSection("azureMaps");

            ATLAS_DOMAIN = mapsConfig["AtlasDomain"];
            AZURE_SUBSCRIPTION_ID = mapsConfig["AzureSubscriptionId"];
            MAPS_CLIENT_ID = mapsConfig["MapsClientId"];
            MAPS_AUTHZ_PRIMARYKEY = mapsConfig["AuthenticationPrimaryKey"];
        }

        private static readonly string ATLAS_DOMAIN = "", AZURE_SUBSCRIPTION_ID = "", MAPS_CLIENT_ID = "", MAPS_AUTHZ_PRIMARYKEY = "";
        private static readonly string AZURE_MAPS_BASE_URL = "https://atlas.microsoft.com";
        private static readonly string AZURE_MAPS_TOKEN_CONTEXT = "https://atlas.microsoft.com/.default";
    }
}
