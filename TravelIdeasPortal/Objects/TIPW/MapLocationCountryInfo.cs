using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace TravelIdeasPortalWeb.Objects.TIPW
{
    /// <summary>
    /// Information about a country presented in <see cref="MapLocationInfo"/>
    /// </summary>
    public class MapLocationCountryInfo
    {
        /// <summary>
        /// Name of the country. 
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; set; }

        /// <summary>
        /// The name written in the native language
        /// </summary>
        [JsonPropertyName("nativeName")]
        public string NativeName { get; set; }

        /// <summary>
        /// ISO 2-letter code
        /// </summary>
        [JsonPropertyName("isoCountryCode")]
        public string IsoCountryCode { get; set; }

        /// <summary>
        /// Name of capital city
        /// </summary>
        [JsonPropertyName("capital")]
        public string CapitalCity { get; set; }

        /// <summary>
        /// Region (usually continent)
        /// </summary>
        [JsonPropertyName("region")]
        public string Region { get; set; }

        /// <summary>
        /// Subregion (a smaller locality within the continent)
        /// </summary>
        [JsonPropertyName("subregion")]
        public string Subregion { get; set; }

        /// <summary>
        /// What are people belonging to this country called?
        /// </summary>
        [JsonPropertyName("demonym")]
        public string Demonymn { get; set; }

        /// <summary>
        /// Timezones this country spans
        /// </summary>
        [JsonPropertyName("timezones")]
        public List<string> Timezones { get; set; }

        /// <summary>
        /// Bordering countries (resolved country names)
        /// </summary>
        [JsonPropertyName("borders")]
        public List<string> BorderCountries { get; set; }

        /// <summary>
        /// Currencies used by this country
        /// </summary>
        [JsonPropertyName("currencies")]
        public List<MapLocationCountryCurrency> Currencies { get; set; }

        /// <summary>
        /// Languages spoken in this country
        /// </summary>
        [JsonPropertyName("languages")]
        public List<MapLocationCountryLanguage> Languages { get; set; }

        /// <summary>
        /// Absolute URI to a restcountries.eu hosted flag of the country
        /// </summary>
        [JsonPropertyName("flag")]
        public string Flag { get; set; }

        /// <summary>
        /// Default constructor
        /// </summary>
        public MapLocationCountryInfo() { }

        /// <summary>
        /// Import a RestCountry
        /// </summary>
        /// <param name="restCountry">Country metadata to import</param>
        public static MapLocationCountryInfo From(Objects.RestCountries.RestCountry restCountry)
        {
            if (restCountry == null)
            {
                return null;
            }

            MapLocationCountryInfo result = new MapLocationCountryInfo()
            {
                Name = restCountry.Name,
                NativeName = restCountry.NativeName,
                IsoCountryCode = restCountry.IsoCountryCode,
                CapitalCity = restCountry.CapitalCity,
                Region = restCountry.Region,
                Subregion = restCountry.Subregion,
                Demonymn = restCountry.Demonymn,
                Timezones = restCountry.Timezones,
                BorderCountries = new List<string>(),
                Currencies = new List<MapLocationCountryCurrency>(),
                Languages = new List<MapLocationCountryLanguage>(),
                Flag = restCountry.Flag
            };

            if ((restCountry.BorderCountries != null) && (restCountry.BorderCountries.Count > 0))
            {
                foreach (string code in restCountry.BorderCountries)
                {
                    Objects.RestCountries.RestCountry country = ServiceClients.RestCountriesClient.GetCountry(code);
                    if (country != null)
                    {
                        result.BorderCountries.Add(country.Name);
                    }
                }
            }

            if ((restCountry.Currencies != null) && (restCountry.Currencies.Count > 0))
            {
                foreach (Objects.RestCountries.RestCountryCurrency currency in restCountry.Currencies)
                {
                    result.Currencies.Add(
                            new MapLocationCountryCurrency()
                            {
                                Code = currency.Code,
                                Name = currency.Name,
                                Symbol = currency.Symbol
                            }
                        );
                }
            }

            if ((restCountry.Languages != null) && (restCountry.Languages.Count > 0))
            {
                foreach (Objects.RestCountries.RestCountryLanguage lang in restCountry.Languages)
                {
                    result.Languages.Add(
                            new MapLocationCountryLanguage()
                            {
                                Name = lang.Name,
                                NativeName = lang.NativeName
                            }
                        );
                }
            }

            return result;
        }
    }
}
