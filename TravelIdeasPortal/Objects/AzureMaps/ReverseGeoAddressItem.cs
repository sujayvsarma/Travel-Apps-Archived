using System.Text.Json.Serialization;

namespace TravelIdeasPortalWeb.Objects.AzureMaps
{
    /// <summary>
    /// A single address entry in ReverseGeoAddressResult. 
    /// Note: We only have those properties that we are interested in!
    /// </summary>
    public class ReverseGeoAddressItem
    {
        /// <summary>
        /// The full country name
        /// </summary>
        [JsonPropertyName("country")]
        public string Country { get; set; }

        /// <summary>
        /// 2-letter ISO country code
        /// </summary>
        [JsonPropertyName("countryCode")]
        public string IsoCountry { get; set; }

        /// <summary>
        /// The state or province name
        /// </summary>
        [JsonPropertyName("countrySubdivision")]
        public string StateOrProvince { get; set; }

        /// <summary>
        /// The city/municipal limits name
        /// </summary>
        [JsonPropertyName("municipality")]
        public string City { get; set; }

    }
}
