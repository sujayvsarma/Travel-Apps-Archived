using System.Text.Json.Serialization;

namespace TravelIdeasPortalWeb.Objects.RestCountries
{
    /// <summary>
    /// Currency for a <see cref="RestCountry"/>
    /// </summary>
    public class RestCountryCurrency
    {
        /// <summary>
        /// Three-letter currency code
        /// </summary>
        [JsonPropertyName("code")]
        public string Code { get; set; }

        /// <summary>
        /// Name of currency
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; set; }

        /// <summary>
        /// UTF-8 symbol of the currency
        /// </summary>
        [JsonPropertyName("symbol")]
        public string Symbol { get; set; }
    }
}
