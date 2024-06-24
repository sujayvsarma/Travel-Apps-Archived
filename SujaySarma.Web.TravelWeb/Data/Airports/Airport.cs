using System.Text.Json.Serialization;

namespace SujaySarma.Web.TravelWeb.Data.Airports
{
    public class Airport
    {
        /// <summary>
        /// Name of the airport
        /// </summary>
        [JsonPropertyName("name")]
        public string? Name { get; set; }

        /// <summary>
        /// Latitude
        /// </summary>
        [JsonPropertyName("lat")]
        public double Latitude { get; set; }

        /// <summary>
        /// Longitude
        /// </summary>
        [JsonPropertyName("lon")]
        public double Longitude { get; set; }

        /// <summary>
        /// ISO Country Code
        /// </summary>
        [JsonPropertyName("country")]
        public string? ISOCountryCode { get; set; }

        /// <summary>
        /// City/municipality limits
        /// </summary>
        [JsonPropertyName("municipality")]
        public string? Municipality { get; set; }

        /// <summary>
        /// IATA code for the airport
        /// </summary>
        [JsonPropertyName("iata")]
        public string? Iata { get; set; }

        /// <summary>
        /// Url to the homepage of this airport (if there is one)
        /// </summary>
        [JsonPropertyName("homepage")]
        public string? HomeUri { get; set; }

        /// <summary>
        /// Absolute URI to the Wikipedia page for this region
        /// </summary>
        [JsonPropertyName("wikipedia")]
        public string? WikipediaUri { get; set; }

        /// <summary>
        /// Distance from originally clicked location
        /// </summary>
        [JsonPropertyName("distanceFromClick")]
        public double DistanceFromLocation { get; set; }
    }
}
