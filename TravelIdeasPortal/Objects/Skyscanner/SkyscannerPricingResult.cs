using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace TravelIdeasPortalWeb.Objects.Skyscanner
{
    /// <summary>
    /// Result returned by pricing API:
    /// https://skyscanner-skyscanner-flight-search-v1.p.rapidapi.com/apiservices/pricing/
    /// </summary>
    public class SkyscannerPricingResult
    {
        /// <summary>
        /// Places in this result
        /// </summary>
        [JsonPropertyName("Places")]
        public List<SkyscannerPricingPlace> Places { get; set; } = new List<SkyscannerPricingPlace>();

        /// <summary>
        /// Agencies and airlines
        /// </summary>
        [JsonPropertyName("Agents")]
        public List<SkyscannerPricingAgent> Agents { get; set; } = new List<SkyscannerPricingAgent>();

        /// <summary>
        /// Airlines
        /// </summary>
        [JsonPropertyName("Carriers")]
        public List<SkyscannerPricingCarrier> Carriers { get; set; } = new List<SkyscannerPricingCarrier>();

        /// <summary>
        /// Segments of travel
        /// </summary>
        [JsonPropertyName("Segments")]
        public List<SkyscannerPricingSegment> Segments { get; set; } = new List<SkyscannerPricingSegment>();

        /// <summary>
        /// Legs in this travel
        /// </summary>
        [JsonPropertyName("Legs")]
        public List<SkyscannerPricingLeg> Legs { get; set; } = new List<SkyscannerPricingLeg>();

        /// <summary>
        /// Itineraries
        /// </summary>
        [JsonPropertyName("Itineraries")]
        public List<SkyscannerPricingItinerary> Itineraries { get; set; } = new List<SkyscannerPricingItinerary>();

        // This structure has properties we do not care about.
    }
}
