using System.Text.Json.Serialization;

namespace TravelIdeasPortalWeb.Objects.TIPW
{
    /// <summary>
    /// Language spoken in a country
    /// </summary>
    public class MapLocationCountryLanguage
    {
        /// <summary>
        /// Name of the language
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; set; }

        /// <summary>
        /// The name written in the native language
        /// </summary>
        [JsonPropertyName("nativeName")]
        public string NativeName { get; set; }
    }
}
