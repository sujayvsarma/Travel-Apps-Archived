using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace TravelIdeasPortalWeb.Objects.Skyscanner
{
    /// <summary>
    /// Pricing option for <see cref="SkyscannerPricingItinerary"/>
    /// </summary>
    public class SkyscannerPricingOption
    {
        /// <summary>
        /// Agents
        /// </summary>
        [JsonPropertyName("Agents")]
        public List<int> Agents { get; set; }

        /// <summary>
        /// Value of the price
        /// </summary>
        [JsonPropertyName("Price")]
        public decimal Price { get; set; }

        /// <summary>
        /// Absolute URI for the customer to make a booking for this option
        /// </summary>
        [JsonPropertyName("DeeplinkUrl")]
        public string BookingURI { get; set; }
    }
}
