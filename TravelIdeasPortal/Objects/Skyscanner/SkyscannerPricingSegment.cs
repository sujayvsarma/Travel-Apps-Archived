using System;
using System.Text.Json.Serialization;

namespace TravelIdeasPortalWeb.Objects.Skyscanner
{
    /// <summary>
    /// A single segment in <see cref="SkyscannerPricingResult"/>
    /// </summary>
    public class SkyscannerPricingSegment
    {
        /// <summary>
        /// Segment ID
        /// </summary>
        [JsonPropertyName("Id")]
        public int Id { get; set; }

        /// <summary>
        /// Point of origin
        /// </summary>
        [JsonPropertyName("OriginStation")]
        public int Origin { get; set; }

        /// <summary>
        /// Destination
        /// </summary>
        [JsonPropertyName("DestinationStation")]
        public int Destination { get; set; }

        /// <summary>
        /// Date/time of departure (time local to user? airport? UTC?)
        /// </summary>
        [JsonPropertyName("DepartureDateTime")]
        public DateTime Departure { get; set; }

        /// <summary>
        /// Date/time of arrival (time local to user? airport? UTC?)
        /// </summary>
        [JsonPropertyName("ArrivalDateTime")]
        public DateTime Arrival { get; set; }

        /// <summary>
        /// Carrier ID
        /// </summary>
        [JsonPropertyName("Carrier")]
        public int CarrierID { get; set; }

        /// <summary>
        /// Duration of trip in minutes
        /// </summary>
        [JsonPropertyName("Duration")]
        public int DurationInMinutes { get; set; }

        /// <summary>
        /// Flight number
        /// </summary>
        [JsonPropertyName("FlightNumber")]
        public string FlightNumber { get; set; }
    }
}
