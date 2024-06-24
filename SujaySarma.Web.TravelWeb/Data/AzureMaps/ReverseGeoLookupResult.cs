using System.Text.Json.Serialization;

namespace SujaySarma.Web.TravelWeb.Data.AzureMaps
{
    /// <summary>
    /// Result for reverse Geo lookup (/search/address/reverse)
    /// </summary>
    internal class ReverseGeoLookupResult
    {
        /// <summary>
        /// Summary result
        /// </summary>
        [JsonPropertyName("summary")]
        public Dictionary<string, int>? Summary { get; set; }

        /// <summary>
        /// Addresses
        /// </summary>
        [JsonPropertyName("addresses")]
        public List<ReverseGeoAddressResult>? Addresses { get; set; }

        /// <summary>
        /// Return the contained address
        /// </summary>
        /// <returns></returns>
        public ReverseGeoAddressItem? GetAddressItem()
        {
            if ((Addresses == null) || (Addresses.Count == 0))
            {
                return null;
            }

            return Addresses[0].Address;
        }
    }

    internal class ReverseGeoAddressResult
    {
        /// <summary>
        /// The actual address
        /// </summary>
        [JsonPropertyName("address")]
        public ReverseGeoAddressItem? Address { get; set; }
    }

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
        public string? Country { get; set; }

        /// <summary>
        /// 2-letter ISO country code
        /// </summary>
        [JsonPropertyName("countryCode")]
        public string? IsoCountry { get; set; }

        /// <summary>
        /// The state or province name
        /// </summary>
        [JsonPropertyName("countrySubdivision")]
        public string? StateOrProvince { get; set; }

        /// <summary>
        /// The city/municipal limits name
        /// </summary>
        [JsonPropertyName("municipality")]
        public string? City { get; set; }

    }
}
