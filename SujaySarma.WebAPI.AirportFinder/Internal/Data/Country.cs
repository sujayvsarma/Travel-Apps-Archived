
using SujaySarma.Data.Files.TokenLimitedFiles.Attributes;

using System.Text.Json.Serialization;

namespace SujaySarma.WebAPI.AirportFinder.Internal.Data
{
    /// <summary>
    /// Denotes a country (OurAirports > countries.csv)
    /// </summary>
    public class Country
    {
        /// <summary>
        /// Numeric global Id for region
        /// </summary>
        [FileField("id")]
        [JsonPropertyName("id")]
        public int Id { get; set; }

        /// <summary>
        /// ISO Country Code
        /// </summary>
        [FileField("code")]
        [JsonPropertyName("code")]
        public string? Code { get; set; }

        /// <summary>
        /// Name of the region
        /// </summary>
        [FileField("name")]
        [JsonPropertyName("name")]
        public string? Name { get; set; }

        /// <summary>
        /// ISO code of the continent
        /// </summary>
        [FileField("continent")]
        [JsonPropertyName("continent")]
        public string? Continent { get; set; }

        /// <summary>
        /// Absolute URI to the Wikipedia page for this region
        /// </summary>
        [FileField("wikipedia_link")]
        [JsonPropertyName("wikipedia")]
        public string? WikipediaUri { get; set; }

        /// <summary>
        /// Keywords
        /// </summary>
        [FileField("keywords")]
        [JsonPropertyName("keywords")]
        public string? Keywords { get; set; }

    }
}
