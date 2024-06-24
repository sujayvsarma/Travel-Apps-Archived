using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace TravelIdeasPortalWeb.Objects.Skyscanner
{
    /// <summary>
    /// Itinerary in <see cref="SkyscannerPricingResult"/>
    /// </summary>
    public class SkyscannerPricingItinerary
    {
        /// <summary>
        /// Id of outbound leg
        /// </summary>
        [JsonPropertyName("OutboundLegId")]
        public string OutboundLegID { get; set; }

        /// <summary>
        /// Id of inbound leg
        /// </summary>
        [JsonPropertyName("InboundLegId")]
        public string InboundLegID { get; set; }

        /// <summary>
        /// Pricing option
        /// </summary>
        [JsonPropertyName("PricingOptions")]
        public List<SkyscannerPricingOption> Pricing { get; set; }
    }
}
