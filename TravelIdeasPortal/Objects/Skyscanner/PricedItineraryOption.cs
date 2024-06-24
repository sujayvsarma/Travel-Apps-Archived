using System;
using System.Collections.Generic;

namespace TravelIdeasPortalWeb.Objects.Skyscanner
{
    /// <summary>
    /// An itinerary option with pricing, flight information and ticketing links
    /// </summary>
    public class PricedItineraryOption
    {

        /// <summary>
        /// A guid for this option to find it later
        /// </summary>
        public Guid Id { get; set; } = Guid.Empty;


        /// <summary>
        /// True if the option features both forward and return flights
        /// </summary>
        public bool IsRoundTrip { get; set; } = false;

        /// <summary>
        /// Number of legs in the forward journey. Direct flights = 1. 
        /// </summary>
        public int NumberOfForwardLegs => ((ForwardSegments == null) ? 0 : ForwardSegments.Count);

        /// <summary>
        /// Number of legs in the return journey. Direct flights = 1. 
        /// If not a roundtrip, will be zero.
        /// </summary>
        public int NumberOfReturnLegs => ((ReturnSegments == null) ? 0 : ReturnSegments.Count);

        /// <summary>
        /// Ticket price value of the trip
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// URL for ticketing this option
        /// </summary>
        public string TicketingUri { get; set; }

        /// <summary>
        /// Agent providing this ticket option. Key is name, value is Logo URI
        /// </summary>
        public KeyValuePair<string, string> Agent { get; set; }

        /// <summary>
        /// List of forward journey segments
        /// </summary>
        public List<PricedItinerarySegment> ForwardSegments { get; set; }

        /// <summary>
        /// The return journey segments. NULL if not a return trip
        /// </summary>
        public List<PricedItinerarySegment> ReturnSegments { get; set; } = null;

        /// <summary>
        /// Total duration (in minutes) of all forward segments
        /// </summary>
        public int TotalOutboundDuration { get; set; } = 0;

        /// <summary>
        /// Total duration (in minutes) of all return segments
        /// </summary>
        public int TotalInboundDuration { get; set; } = 0;
    }
}
