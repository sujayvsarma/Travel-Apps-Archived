
using SujaySarma.Web.TravelWeb.Data.Countries;

using System.IO;
using System.Reflection;
using System.Text.Json;

namespace SujaySarma.Web.TravelWeb.ApiClients
{
    internal static class CountriesApiClient
    {

        /// <summary>
        /// Get a single country given its ISO country code
        /// </summary>
        /// <param name="isoCountryCode">ISO country code of the country</param>
        /// <returns>Country info or NULL</returns>
        public static Country? GetCountry(string isoCountryCode)
            => _countries?.FirstOrDefault(c => ((c.ISOCountryCode != null) && c.ISOCountryCode.Equals(isoCountryCode, StringComparison.InvariantCultureIgnoreCase)));


        static CountriesApiClient()
        {
            string json = Path.Combine(AppContext.BaseDirectory, "Data", "Countries", "countries.json");
            _countries = JsonSerializer.Deserialize<List<Country>>(File.ReadAllText(json));
        }
        private static List<Country>? _countries;


    }
}
