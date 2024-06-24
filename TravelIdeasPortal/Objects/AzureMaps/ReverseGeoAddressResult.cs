using System.Text.Json.Serialization;

namespace TravelIdeasPortalWeb.Objects.AzureMaps
{
    public class ReverseGeoAddressResult
    {
        /// <summary>
        /// The actual address
        /// </summary>
        [JsonPropertyName("address")]
        public ReverseGeoAddressItem Address { get; set; }
    }
}
