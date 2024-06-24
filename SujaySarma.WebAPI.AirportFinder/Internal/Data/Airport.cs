
using SujaySarma.Data.Files.TokenLimitedFiles.Attributes;

using System.Text.Json.Serialization;

namespace SujaySarma.WebAPI.AirportFinder.Internal.Data
{
    /// <summary>
    ///  Denotes an airport (OurAirports > airports.csv)
    /// </summary>
    public class Airport
    {
        /// <summary>
        /// Numeric global Id for region
        /// </summary>
        [FileField(0, "id")]
        [JsonPropertyName("id")]
        public int? Id { get; set; }

        /// <summary>
        /// ICAO code for the airport
        /// </summary>
        [FileField(1, "ident")]
        [JsonPropertyName("icao")]
        public string? Icao { get; set; }

        /// <summary>
        /// Type of airport
        /// </summary>
        [FileField(2, "type")]
        [JsonPropertyName("type")]
        public string? Type { get; set; }

        /// <summary>
        /// Name of the airport
        /// </summary>
        [FileField(3, "name")]
        [JsonPropertyName("name")]
        public string? Name { get; set; }

        /// <summary>
        /// Latitude
        /// </summary>
        [FileField(4, "latitude_deg")]
        [JsonPropertyName("lat")]
        public decimal? Latitude { get; set; }

        /// <summary>
        /// Longitude
        /// </summary>
        [FileField(5, "longitude_deg")]
        [JsonPropertyName("lon")]
        public decimal? Longitude { get; set; }

        /// <summary>
        /// Elevation in feet
        /// </summary>
        [FileField(6, "elevation_ft")]
        [JsonPropertyName("elev")]
        public int? Elevation { get; set; }

        /// <summary>
        /// Continent
        /// </summary>
        [FileField(7, "continent")]
        [JsonPropertyName("continent")]
        public string? Continent { get; set; }

        /// <summary>
        /// ISO country code
        /// </summary>
        [FileField(8, "iso_country")]
        [JsonPropertyName("country")]
        public string? Country { get; set; }

        /// <summary>
        /// ISO code for region or state
        /// </summary>
        [FileField(9, "iso_region")]
        [JsonPropertyName("region")]
        public string? State { get; set; }

        /// <summary>
        /// Municipality
        /// </summary>
        [FileField(10, "municipality")]
        [JsonPropertyName("municipality")]
        public string? Municipality { get; set; }

        /// <summary>
        /// Has scheduled services (False for private or part-time airports)
        /// </summary>
        [FileField(11, "scheduled_service")]
        [JsonPropertyName("hasScheduledService")]
        public bool? HasScheduledService { get; set; }

        /// <summary>
        /// GPS code
        /// </summary>
        [FileField(12, "gps_code")]
        [JsonPropertyName("gps")]
        public string? GPS { get; set; }

        /// <summary>
        /// IATA code for the airport
        /// </summary>
        [FileField(13, "iata_code")]
        [JsonPropertyName("iata")]
        public string? Iata { get; set; }

        /// <summary>
        /// Local code (usually same as the GPS)
        /// </summary>
        [FileField(14, "local_code")]
        [JsonPropertyName("localCode")]
        public string? LocationCode { get; set; }

        /// <summary>
        /// Url to the homepage of this airport (if there is one)
        /// </summary>
        [FileField(15, "home_link")]
        [JsonPropertyName("homepage")]
        public string? HomeUri { get; set; }

        /// <summary>
        /// Absolute URI to the Wikipedia page for this region
        /// </summary>
        [FileField(16, "wikipedia_link")]
        [JsonPropertyName("wikipedia")]
        public string? WikipediaUri { get; set; }

        /// <summary>
        /// Common search keywords/names for this region
        /// </summary>
        [FileField(17, "keywords")]
        [JsonPropertyName("keywords")]
        public string? Keywords { get; set; }

    }
}
