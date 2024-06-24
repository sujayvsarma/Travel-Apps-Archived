using SujaySarma.Sdk.Core;
using SujaySarma.Sdk.DataSources.AzureTables.Attributes;

using System;

namespace TravelIdeasPortalWeb.Objects.DB
{
    /// <summary>
    /// Log of what happened in an itinerary build-out.
    /// </summary>
    [Table("SessionLog")]
    public class SessionLog
    {
        #region Keys

        /// <summary>
        /// Partition key
        /// </summary>
        [PartitionKey]
        public string PartitionKey { get; set; }

        /// <summary>
        /// A unique string (generated using the UniqueStrings library). The user can use this 
        /// later to retrieve these results
        /// </summary>
        [RowKey]
        public string RowKey { get; set; }

        #endregion

        #region Properties

        /// <summary>
        /// ASP.NET Core Session ID
        /// </summary>
        [TableColumn("AspNetCoreSessionId")]
        public string AspNetCoreSessionId { get; set; }

        /// <summary>
        /// Time stamp of when the session started
        /// </summary>
        [TableColumn("SessionStartTime")]
        public DateTime SessionStart { get; set; }

        /// <summary>
        /// IPv4 address
        /// </summary>
        [TableColumn("ipv4")]
        public string IPv4 { get; set; }

        /// <summary>
        /// Geolocated latitude
        /// </summary>
        [TableColumn("geo_lat")]
        public double GeoLocLat { get; set; }

        /// <summary>
        /// Geolocated longitude
        /// </summary>
        [TableColumn("geo_lon")]
        public double GeoLocLon { get; set; }

        /// <summary>
        /// Geolocated country ISO code
        /// </summary>
        [TableColumn("geo_country_iso")]
        public string GeoCountryISO { get; set; }

        /// <summary>
        /// Geolocated country name
        /// </summary>
        [TableColumn("geo_country_name")]
        public string GeoCountryName { get; set; }

        /// <summary>
        /// Geolocated country's currency name
        /// </summary>
        [TableColumn("geo_currency_name")]
        public string GeoCurrencyName { get; set; }

        /// <summary>
        /// Geolocated country's currency symbol
        /// </summary>
        [TableColumn("geo_currency_glyph")]
        public string GeoCurrencySymbol { get; set; }

        /// <summary>
        /// The itinerary as planned by the user
        /// </summary>
        [TableColumn("iti_planned")]
        public string PlannedItinerary { get; set; }

        #endregion

        #region Non-DB properties

        /// <summary>
        /// TIPW-optimized itinerary
        /// </summary>
        public string OptimizedItinerary { get; set; }

        /// <summary>
        /// (post optimization) Results-shown JSON
        /// </summary>
        public string TicketResultJson { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Instantiate
        /// </summary>
        public SessionLog() { }

        /// <summary>
        /// Create a new record
        /// </summary>
        /// <returns>SessionLog</returns>
        public static SessionLog New()
        {
            return new SessionLog()
            {
                PartitionKey = "flighthoteltravelideas.com",
                RowKey = UniqueStrings.Generate().ToUpper()
            };
        }

        #endregion

    }
}
