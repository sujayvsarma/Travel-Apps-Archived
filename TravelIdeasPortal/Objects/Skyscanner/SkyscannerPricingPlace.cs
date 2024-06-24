using System.Text.Json.Serialization;

namespace TravelIdeasPortalWeb.Objects.Skyscanner
{
    /// <summary>
    /// A place in the <see cref="SkyscannerPricingResult"/> structure
    /// </summary>
    public class SkyscannerPricingPlace
    {
        /// <summary>
        /// Place ID
        /// </summary>
        [JsonPropertyName("Id")]
        public int Id { get; set; }

        /// <summary>
        /// IATA code of the place's airport
        /// </summary>
        [JsonPropertyName("Code")]
        public string IATA { get; set; }
    }

}
