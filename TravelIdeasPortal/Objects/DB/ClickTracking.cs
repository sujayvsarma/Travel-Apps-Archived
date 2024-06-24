using SujaySarma.Sdk.DataSources.AzureTables.Attributes;

using System;

namespace TravelIdeasPortalWeb.Objects.DB
{
    /// <summary>
    /// Provides outbound click tracking
    /// </summary>
    [Table("ClickTracking")]
    public class ClickTracking
    {

        #region Keys

        /// <summary>
        /// The row key from the SessionLog table
        /// </summary>
        [PartitionKey]
        public string SessionLogRowKey { get; set; }

        /// <summary>
        /// Unique ID for this record
        /// </summary>
        [RowKey]
        public Guid RowKey { get; set; }

        #endregion

        #region Properties

        /// <summary>
        /// Origin (IATA)
        /// </summary>
        [TableColumn("origin")]
        public string Origin { get; set; }

        /// <summary>
        /// Destination (IATA)
        /// </summary>
        [TableColumn("destination")]
        public string Destination { get; set; }

        /// <summary>
        /// The currency the ticket value is shown in. (3-letter code)
        /// We will only have this in the Skyscanner-provided currency
        /// </summary>
        [TableColumn("ticketValue_currency")]
        public string TicketValueCurrency { get; set; }

        /// <summary>
        /// Monetary value of the ticket (opportunity lost)
        /// </summary>
        [TableColumn("ticketValue")]
        public decimal TicketValue { get; set; }

        /// <summary>
        /// The agent the "Book Tickets" button was clicked on
        /// </summary>
        [TableColumn("clickAgent")]
        public string WinningAgent { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Instantiate
        /// </summary>
        public ClickTracking() { }

        /// <summary>
        /// Create a new record
        /// </summary>
        /// <param name="sessionId">SessionLog.RowKey</param>
        /// <returns>ClickTracking</returns>
        public static ClickTracking New(string sessionId)
        {
            return new ClickTracking()
            {
                SessionLogRowKey = sessionId,
                RowKey = Guid.NewGuid()
            };
        }

        #endregion

    }
}
