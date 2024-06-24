namespace TravelIdeasPortalWeb.Objects.Skyscanner
{
    /// <summary>
    /// An airport in the SkyscannerPricingRequest
    /// </summary>
    public class SkyscannerPricingRequestHalt
    {
        /// <summary>
        /// IATA code for this airport
        /// </summary>
        public string Iata { get; set; }

        /// <summary>
        /// The ISO country code for this airport
        /// </summary>
        public string IsoCountryCode { get; set; }

        /// <summary>
        /// The Skyscanner ID - Filled in by the GetTicketableQuotes method
        /// </summary>
        public string SkyId { get; set; } = null;
    }
}
