
using SujaySarma.Data.Files.TokenLimitedFiles.Attributes;

using System.Text.Json.Serialization;

namespace SujaySarma.WebAPI.AirportFinder.Internal.Data
{
    /// <summary>
    /// Denotes a region (OurAirports > regions.csv)
    /// </summary>
    public class Region
    {
        /// <summary>
        /// Numeric global Id for region
        /// </summary>
        [FileField(0, "id")]
        [JsonPropertyName("id")]
        public int Id { get; set; }

        /// <summary>
        /// "ISO Country Code - Local Code"
        /// </summary>
        [FileField(1, "code")]
        [JsonPropertyName("code")]
        public string? Code { get; set; }

        /// <summary>
        /// Local Code
        /// </summary>
        [FileField(2, "local_code")]
        [JsonPropertyName("localCode")]
        public string? LocalCode { get; set; }

        /// <summary>
        /// Name of the region
        /// </summary>
        [FileField(3, "name")]
        [JsonPropertyName("name")]
        public string? Name { get; set; }

        /// <summary>
        /// ISO code of the continent
        /// </summary>
        [FileField(4, "continent")]
        [JsonPropertyName("continent")]
        public string? Continent { get; set; }

        /// <summary>
        /// ISO Country code
        /// </summary>
        [FileField(5, "iso_country")]
        [JsonPropertyName("country")]
        public string? Country { get; set; }

        /// <summary>
        /// Absolute URI to the Wikipedia page for this region
        /// </summary>
        [FileField(6, "wikipedia_link")]
        [JsonPropertyName("wikipedia")]
        public string? WikipediaUri { get; set; }

        /// <summary>
        /// Common search keywords/names for this region
        /// </summary>
        [FileField(7, "keywords")]
        [JsonPropertyName("keywords")]
        public string? Keywords { get; set; }

    }
}
