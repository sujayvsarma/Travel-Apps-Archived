using System;

namespace TravelIdeasPortalWeb.Objects.Skyscanner
{
    /// <summary>
    /// A segment in a priced itinerary
    /// </summary>
    public class PricedItinerarySegment
    {
        /// <summary>
        /// Point of origin
        /// </summary>
        public ItineraryHalt Origin { get; set; }

        /// <summary>
        /// Final destination
        /// </summary>
        public ItineraryHalt Destination { get; set; }

        /// <summary>
        /// Date/time of departure (local time of Origin)
        /// </summary>
        public DateTime Departure { get; set; }

        /// <summary>
        /// Date/time of arrival (local time of Destination)
        /// </summary>
        public DateTime Arrival { get; set; }

        /// <summary>
        /// Flight duration in minutes
        /// </summary>
        public int Duration { get; set; }

        /// <summary>
        /// The flight
        /// </summary>
        public ItineraryFlight Flight { get; set; }

    }
}
