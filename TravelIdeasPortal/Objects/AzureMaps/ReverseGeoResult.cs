using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace TravelIdeasPortalWeb.Objects.AzureMaps
{
    /// <summary>
    /// Result for reverse Geo lookup (/search/address/reverse)
    /// </summary>
    public class ReverseGeoResult
    {
        /// <summary>
        /// Summary result
        /// </summary>
        [JsonPropertyName("summary")]
        public Dictionary<string, int> Summary { get; set; }

        /// <summary>
        /// Addresses
        /// </summary>
        [JsonPropertyName("addresses")]
        public List<ReverseGeoAddressResult> Addresses { get; set; }
    }
}
