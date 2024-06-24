using System.Text.Json.Serialization;

namespace TravelIdeasPortalWeb.Objects.Skyscanner
{
    /// <summary>
    /// A carrier in <see cref="SkyscannerPricingResult"/>
    /// </summary>
    public class SkyscannerPricingCarrier
    {
        /// <summary>
        /// Carrier ID
        /// </summary>
        [JsonPropertyName("Id")]
        public int Id { get; set; }

        /// <summary>
        /// IATA code of the carrier
        /// </summary>
        [JsonPropertyName("Code")]
        public string IATA { get; set; }

        /// <summary>
        /// Name of the airline
        /// </summary>
        [JsonPropertyName("Name")]
        public string Name { get; set; }

        /// <summary>
        /// URI to the airline's logo (absolute, PNG)
        /// </summary>
        [JsonPropertyName("ImageUrl")]
        public string LogoURI { get; set; }

        /// <summary>
        /// User-displayable IATA code (*sometimes* different from <see cref="IATA"/>).
        /// </summary>
        [JsonPropertyName("DisplayCode")]
        public string DisplayIATA { get; set; }
    }
}
