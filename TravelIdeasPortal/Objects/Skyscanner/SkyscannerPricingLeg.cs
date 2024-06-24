using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace TravelIdeasPortalWeb.Objects.Skyscanner
{
    /// <summary>
    /// A leg in the <see cref="SkyscannerPricingResult"/>
    /// </summary>
    public class SkyscannerPricingLeg
    {
        /// <summary>
        /// Leg ID
        /// </summary>
        [JsonPropertyName("Id")]
        public string Id { get; set; }

        /// <summary>
        /// Ids of segments
        /// </summary>
        [JsonPropertyName("SegmentIds")]
        public List<int> Segments { get; set; }
    }
}
