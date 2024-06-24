using System.Text.Json.Serialization;

namespace TravelIdeasPortalWeb.Objects.Skyscanner
{
    /// <summary>
    /// A travel agency or airline that is providing the pricing
    /// </summary>
    public class SkyscannerPricingAgent
    {
        /// <summary>
        /// Agent ID
        /// </summary>
        [JsonPropertyName("Id")]
        public int Id { get; set; }

        /// <summary>
        /// Name of the agency/airline
        /// </summary>
        [JsonPropertyName("Name")]
        public string Name { get; set; }

        /// <summary>
        /// URI to the agency's logo (absolute)
        /// </summary>
        [JsonPropertyName("ImageUrl")]
        public string LogoURI { get; set; }

        /// <summary>
        /// Type of agency (Airline, TravelAgent)
        /// </summary>
        [JsonPropertyName("Type")]
        public string Type { get; set; }
    }
}
