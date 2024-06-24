using System.Text.Json.Serialization;

namespace TravelIdeasPortalWeb.Objects.TIPW
{
    /// <summary>
    /// Cheap destination for a MapLocation information
    /// </summary>
    public class MapLocationCheapDestination
    {
        /// <summary>
        /// Code for the place
        /// </summary>
        [JsonPropertyName("iata")]
        public string IATA { get; set; }

        /// <summary>
        /// Name of the place
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; set; }

        /// <summary>
        /// The lowest price to this destination
        /// </summary>
        [JsonPropertyName("lowestPrice")]
        public decimal LowestPrice { get; set; } = 0;

        /// <summary>
        /// Whether direct flights are available
        /// </summary>
        [JsonPropertyName("hasDirectFlights")]
        public bool DirectFlightsAvailable { get; set; } = true;
    }
}
