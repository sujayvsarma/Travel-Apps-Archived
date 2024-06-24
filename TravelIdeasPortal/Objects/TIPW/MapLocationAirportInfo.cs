using System.Text.Json.Serialization;

namespace TravelIdeasPortalWeb.Objects.TIPW
{
    /// <summary>
    /// Airport information in MapLocationInfo
    /// </summary>
    public class MapLocationAirportInfo
    {
        /// <summary>
        /// Name of the airport
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; set; }

        /// <summary>
        /// Name of the country
        /// </summary>
        [JsonPropertyName("country")]
        public string CountryName { get; set; }

        /// <summary>
        /// ISO country code
        /// </summary>
        [JsonPropertyName("isoCountry")]
        public string ISOCountryCode { get; set; }

        /// <summary>
        /// Name of region or state
        /// </summary>
        [JsonPropertyName("region")]
        public string State { get; set; }

        /// <summary>
        /// City/municipality limits
        /// </summary>
        [JsonPropertyName("municipality")]
        public string Municipality { get; set; }

        /// <summary>
        /// GPS code
        /// </summary>
        [JsonPropertyName("gps")]
        public string GPS { get; set; }

        /// <summary>
        /// IATA code for the airport
        /// </summary>
        [JsonPropertyName("iata")]
        public string Iata { get; set; }

        /// <summary>
        /// Url to the homepage of this airport (if there is one)
        /// </summary>
        [JsonPropertyName("homepage")]
        public string HomeUri { get; set; }

        /// <summary>
        /// Absolute URI to the Wikipedia page for this region
        /// </summary>
        [JsonPropertyName("wikipedia")]
        public string WikipediaUri { get; set; }

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
        /// Distance from originally clicked location
        /// </summary>
        [JsonPropertyName("distanceFromClick")]
        public double DistanceFromLocation { get; set; }
    }
}
