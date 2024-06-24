namespace TravelIdeasPortalWeb.Objects.TPC
{
    /// <summary>
    /// This is a structure that forms a header on the TicketingUX view
    /// </summary>
    public class TUXHeader
    {
        /// <summary>
        /// The date of departure
        /// </summary>
        public string Date { get; set; } = null;

        /// <summary>
        /// The "Origin - Destination" string, ready to go
        /// </summary>
        public string OrigDestString { get; set; } = null;

        /// <summary>
        /// Number of options
        /// </summary>
        public int NumberOfOptions { get; set; } = 0;
    }
}
