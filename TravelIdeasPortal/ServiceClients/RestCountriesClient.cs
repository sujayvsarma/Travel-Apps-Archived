using SujaySarma.Sdk.RestApi;

using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;

using TravelIdeasPortalWeb.Objects.RestCountries;

namespace TravelIdeasPortalWeb.ServiceClients
{
    public static class RestCountriesClient
    {
        /// <summary>
        /// Find a country with its name or ISO country code
        /// </summary>
        /// <param name="nameOrIsoCode">The full name or ISO country code of the country to find</param>
        /// <returns>The country if found, else NULL</returns>
        public static RestCountry GetCountry(string nameOrIsoCode)
        {
            foreach (RestCountry country in _countriesList)
            {
                if ((country.Name.Equals(nameOrIsoCode, StringComparison.InvariantCultureIgnoreCase)) || (country.IsoCountryCode.Equals(nameOrIsoCode, StringComparison.InvariantCultureIgnoreCase)))
                {
                    return country;
                }
            }

            return null;
        }


        static RestCountriesClient()
        {
            RestApiClient client = new RestApiClient(new Uri("https://restcountries.eu/rest/v2/all"));
            HttpResponseMessage responseMessage = client.Get();
            _countriesList = (responseMessage.IsSuccessStatusCode
                    ? JsonSerializer.Deserialize<List<RestCountry>>(responseMessage.Content.ReadAsStringAsync().Result)
                    : new List<RestCountry>()
                );
        }
        private static readonly List<RestCountry> _countriesList = null;

    }
}
