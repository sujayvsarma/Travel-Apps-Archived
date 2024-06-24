using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace TravelIdeasPortalWeb.Objects.RestCountries
{
    /// <summary>
    /// Result of the restcountries.eu /all endpoint. We are only interested in fixed properties of the country. 
    /// Other properties are ignored.
    /// </summary>
    public class RestCountry
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
        [JsonPropertyName("alpha2Code")]
        public string IsoCountryCode { get; set; }

        /// <summary>
        /// 3-letter country code
        /// </summary>
        [JsonPropertyName("alpha3Code")]
        public string ExtCountryCode { get; set; }

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
        /// Bordering countries (this is the <see cref="ExtCountryCode"/>)
        /// </summary>
        [JsonPropertyName("borders")]
        public List<string> BorderCountries { get; set; }

        /// <summary>
        /// Currencies used by this country
        /// </summary>
        [JsonPropertyName("currencies")]
        public List<RestCountryCurrency> Currencies { get; set; }

        /// <summary>
        /// Languages spoken in this country
        /// </summary>
        [JsonPropertyName("languages")]
        public List<RestCountryLanguage> Languages { get; set; }

        /// <summary>
        /// Absolute URI to a restcountries.eu hosted flag of the country
        /// </summary>
        [JsonPropertyName("flag")]
        public string Flag { get; set; }
    }
}
