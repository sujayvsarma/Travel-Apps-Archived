namespace TravelIdeasPortalWeb.Objects.Skyscanner
{
    /// <summary>
    /// The airline and flightnumber with the logo for a flight leg
    /// </summary>
    public class ItineraryFlight
    {
        /// <summary>
        /// IATA code of the carrier
        /// </summary>
        public string IATA { get; set; }

        /// <summary>
        /// Name of the airline
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// URI to the airline's logo (absolute, PNG)
        /// </summary>
        public string LogoURI { get; set; }

        /// <summary>
        /// The flight number
        /// </summary>
        public string FlightNumber { get; set; }
    }
}
