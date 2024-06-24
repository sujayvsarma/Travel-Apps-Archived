namespace TravelIdeasPortalWeb.Objects.Skyscanner
{
    /// <summary>
    /// A "halt" point or airport
    /// </summary>
    public class ItineraryHalt
    {
        /// <summary>
        /// IATA code of the airport
        /// </summary>
        public string IATA { get; set; }

        /// <summary>
        /// Name of the location
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 2-letter ISO country code
        /// </summary>
        public string IsoCountryCode { get; set; }

        /// <summary>
        /// The full country name in English
        /// </summary>
        public string CountryName { get; set; }
    }
}
