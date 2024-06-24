using SujaySarma.Sdk.RestApi;

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;

using TravelIdeasPortalWeb.Objects.OurAirport;

namespace TravelIdeasPortalWeb.ServiceClients
{
    /// <summary>
    /// Client to interact with our OurAirportsApi service
    /// </summary>
    internal static class OurAirportsApiClient
    {
        /// <summary>
        /// Get the airports nearest to the coordinates
        /// </summary>
        /// <param name="lat">Latitude</param>
        /// <param name="lon">Longitude</param>
        /// <param name="count">Number of airports to return</param>
        /// <returns>List of OurAirportInfoResult with the combined results</returns>
        public static List<OurAirportInfoResult> GetNearestAirports(double lat, double lon, int count)
        {
            RestApiClient client = new RestApiClient(new Uri($"{BASE_URL}/nearest/{lat.ToString("N5")},{lon.ToString("N5")}?iataOnly=true&expand=true&max={count}"));
            HttpResponseMessage responseMessage = client.Get();
            if (responseMessage.IsSuccessStatusCode)
            {
                JsonResultFromApi<Airport> result = JsonSerializer.Deserialize<JsonResultFromApi<Airport>>(responseMessage.Content.ReadAsStringAsync().Result);
                if ((!result.IsError) && (result.Count > 0))
                {
                    List<OurAirportInfoResult> list = new List<OurAirportInfoResult>();

                    Dictionary<string, object> lookup = result.ExtensionResults;
                    Dictionary<string, Country> countries = JsonSerializer.Deserialize<Dictionary<string, Country>>(((JsonElement)lookup["countries"]).GetRawText());
                    Dictionary<string, Region> regions = JsonSerializer.Deserialize<Dictionary<string, Region>>(((JsonElement)lookup["regions"]).GetRawText());

                    foreach (Airport airport in result.ResultObject)
                    {
                        list.Add(
                                new OurAirportInfoResult()
                                {
                                    Airport = airport,
                                    Country = countries[airport.Country],
                                    StateOrProvince = regions[airport.State]
                                }
                            );
                    }

                    return list;
                }
            }

            return null;
        }

        /// <summary>
        /// Search for an airport
        /// </summary>
        /// <param name="nameOrIata">IATA code, ICAO code, name of airport, or one of the keywords</param>
        /// <returns>OurAirportInfoResult with the result</returns>
        public static OurAirportInfoResult AirportSearch(string nameOrIata, bool expandResults = false)
        {
            return airportResults.GetOrAdd(nameOrIata,
                    (ni) =>
                    {
                        RestApiClient client = new RestApiClient(new Uri($"{BASE_URL}/airport/{ni}?expand={expandResults.ToString()}"));
                        HttpResponseMessage responseMessage = client.Get();
                        if (responseMessage.IsSuccessStatusCode)
                        {
                            JsonResultFromApi<Airport> result = JsonSerializer.Deserialize<JsonResultFromApi<Airport>>(responseMessage.Content.ReadAsStringAsync().Result);
                            if ((!result.IsError) && (result.Count > 0))
                            {
                                Airport airport = result.ResultObject.First();
                                Dictionary<string, object> lookup = result.ExtensionResults;
                                return new OurAirportInfoResult()
                                {
                                    Airport = airport,

                                    // we will have these results only if expandResults was true
                                    Country = (expandResults ? getDictionaryFromJsonElement<Country>((JsonElement)lookup["countries"])[airport.Country] : null),
                                    StateOrProvince = (expandResults ? getDictionaryFromJsonElement<Region>((JsonElement)lookup["regions"])[airport.State] : null)
                                };
                            }
                        }

                        return null;
                    }
                );

            static Dictionary<string, T> getDictionaryFromJsonElement<T>(JsonElement element)
                => JsonSerializer.Deserialize<Dictionary<string, T>>(element.GetRawText());
        }
        private static readonly ConcurrentDictionary<string, OurAirportInfoResult> airportResults = new ConcurrentDictionary<string, OurAirportInfoResult>();


        public static readonly string BASE_URL = "https://ourairportapi.com";
    }
}
