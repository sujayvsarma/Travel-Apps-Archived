using System.Text.Json.Serialization;

namespace TravelIdeasPortalWeb.Objects.OurAirport
{
    /// <summary>
    /// Denotes a country (OurAirports > countries.csv)
    /// </summary>
    public class Country
    {
        /// <summary>
        /// ISO Country Code
        /// </summary>
        [JsonPropertyName("code")]
        public string Code { get; set; }

        /// <summary>
        /// Name of the region
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; set; }

    }
}
