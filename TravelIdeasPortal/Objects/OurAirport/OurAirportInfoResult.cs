using System.Text.Json.Serialization;

namespace TravelIdeasPortalWeb.Objects.OurAirport
{
    /// <summary>
    /// This is the structure we output from the OurAirportsApiClient
    /// </summary>
    public class OurAirportInfoResult
    {
        /// <summary>
        /// The airport
        /// </summary>
        [JsonPropertyName("airport")]
        public Airport Airport { get; set; }

        /// <summary>
        /// The state/province the airport is in
        /// </summary>
        [JsonPropertyName("state")]
        public Region StateOrProvince { get; set; }

        /// <summary>
        /// The country the airport is in
        /// </summary>
        [JsonPropertyName("country")]
        public Country Country { get; set; }
    }
}
