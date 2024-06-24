using Microsoft.AspNetCore.Http;

using SujaySarma.Sdk.AspNetCore.Mvc;
using SujaySarma.Sdk.DataSources.AzureTables;

using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Text.Json;

using TravelIdeasPortalWeb.Objects;
using TravelIdeasPortalWeb.Objects.DB;
using TravelIdeasPortalWeb.Objects.RestCountries;

namespace TravelIdeasPortalWeb.ServiceClients
{
    /// <summary>
    /// Client for interaction with Azure TableStorage
    /// </summary>
    public static class CosmosClient
    {

        /// <summary>
        /// Create a click-tracking record
        /// </summary>
        /// <param name="sessionUniqueId">Unique ID value generated for the session</param>
        /// <param name="origin">IATA for origin</param>
        /// <param name="destination">IATA for destination</param>
        /// <param name="currency">Currency ISO code</param>
        /// <param name="value">Value of ticket</param>
        /// <param name="wonBy">Winning agent NAME</param>
        public static void TrackOutbound(string sessionUniqueId, string origin, string destination, string currency, decimal value, string wonBy)
        {
            ClickTracking ct = ClickTracking.New(sessionUniqueId);
            ct.Origin = origin;
            ct.Destination = destination;
            ct.TicketValueCurrency = currency;
            ct.TicketValue = value;
            ct.WinningAgent = wonBy;

            //HttpContext.Session.SetString("ct", System.Text.Json.JsonSerializer.Serialize(ct));
            //GetDataSource<ClickTracking>().Insert(new ClickTracking[] { ct });
        }

        /// <summary>
        /// Create a new session record
        /// </summary>
        /// <param name="aspNetSessionId">The ASP.NET Session ID</param>
        /// <param name="ipv4">IPv4 address</param>
        /// <param name="geo">Coordinates</param>
        /// <param name="geoCountryCurrency">Geolocated country and currency information</param>
        /// <returns>The row unique Id</returns>
        public static string CreateNewSession(HttpContext httpContext, string aspNetSessionId, string ipv4, Coordinate geo, RestCountry geoCountryCurrency)
        {
            //AzureTablesDataSource ds = GetDataSource<SessionLog>();
            //SessionLog[] exists = ds.Select<SessionLog>("flighthoteltravelideas.com", otherFilters: $"(AspNetCoreSessionId eq '{aspNetSessionId}')").ToArray();
            //if (exists.Length > 0)
            //{
            //    return exists[0].RowKey;
            //}

            SessionLog log = SessionLog.New();
            log.AspNetCoreSessionId = aspNetSessionId;
            log.SessionStart = DateTime.UtcNow;
            log.IPv4 = ipv4;
            log.GeoLocLat = geo.Latitude;
            log.GeoLocLon = geo.Longitude;
            log.GeoCountryISO = geoCountryCurrency.IsoCountryCode;
            log.GeoCountryName = geoCountryCurrency.Name;
            log.GeoCurrencyName = geoCountryCurrency.Currencies[0].Code;
            log.GeoCurrencySymbol = geoCountryCurrency.Currencies[0].Symbol;
            log.PlannedItinerary = null;
            log.OptimizedItinerary = null;
            log.TicketResultJson = null;

            httpContext.Session.SetString("_mapsSession", JsonSerializer.Serialize(log));
            //GetDataSource<SessionLog>().Insert(new SessionLog[] { log });

            return log.RowKey;
        }

        /// <summary>
        /// Update a session record
        /// </summary>
        /// <param name="rowUniqueId">Unique ID generated at CreateNewSession()</param>
        /// <param name="planned">User-planned itinerary</param>
        /// <param name="optimized">Logic-optimized itinerary</param>
        /// <param name="tickets">Tickets result returned by the Skyscanner API</param>
        public static void UpdateSession(HttpContext httpContext, string rowUniqueId, string planned, string optimized, string tickets)
        {
            //AzureTablesDataSource ds = GetDataSource<SessionLog>();
            //SessionLog log = ds.Select<SessionLog>("flighthoteltravelideas.com", rowUniqueId).ToArray()[0];

            SessionLog log = GetSession(httpContext, rowUniqueId, true);
            log.PlannedItinerary = planned;

            httpContext.Session.SetString("_mapsSession", JsonSerializer.Serialize(log));
            //ds.Update(new SessionLog[] { log });

            // optimized & tickets go into files because of the 32/64KB rowsize limit on Azure Tables
            if (!string.IsNullOrWhiteSpace(optimized))
            {
                FileServicesClient.StoreFile(rowUniqueId, "iti_optimized", optimized);
            }
            if (!string.IsNullOrWhiteSpace(tickets))
            {
                FileServicesClient.StoreFile(rowUniqueId, "iti_tickets", tickets);
            }
        }

        /// <summary>
        /// Get a previously saved session
        /// </summary>
        /// <param name="rowUniqueId">Unique ID generated at CreateNewSession()</param>
        /// <returns>SessionLog or NULL</returns>
        public static SessionLog GetSession(HttpContext httpContext, string rowUniqueId, bool loadFiles = false)
        {
            //AzureTablesDataSource ds = GetDataSource<SessionLog>();
            //SessionLog[] exists = ds.Select<SessionLog>("flighthoteltravelideas.com", rowUniqueId).ToArray();

            string prevSerialized = httpContext.Session.GetString("_mapsSession");
            if (string.IsNullOrWhiteSpace(prevSerialized))
            {
                return null;
            }

            SessionLog log = JsonSerializer.Deserialize<SessionLog>(prevSerialized);
            if (log != default)
            {
                //SessionLog log = exists;

                if (loadFiles)
                {
                    // optimized & tickets are in files because of the 32/64KB rowsize limit on Azure Tables
                    log.OptimizedItinerary = FileServicesClient.ReadFile(rowUniqueId, "iti_optimized");
                    log.TicketResultJson = FileServicesClient.ReadFile(rowUniqueId, "iti_tickets");
                }

                return log;
            }

            return null;
        }

        /// <summary>
        /// Try to get a session using its IP address. Used to reduce the number of sessions a user creates
        /// </summary>
        /// <param name="ipAddress">Client IP address in IPv4 format</param>
        /// <returns>A previous session within the Session Timeout threshold</returns>
        public static SessionLog TryGetExistingSession(HttpContext httpContext, string ipAddress)
        {
            // Session Timeout is 30 mins (Startup.cs > Line 73)
            string previousSessionThreshold = DateTime.UtcNow.AddMinutes(-29).ToString("yyyy-MM-ddTHH:mm:ss.msZ");

            //AzureTablesDataSource ds = GetDataSource<SessionLog>();
            //SessionLog[] sessions = ds.Select<SessionLog>("flighthoteltravelideas.com", otherFilters: $"(SessionStartTime ge datetime'{previousSessionThreshold}') and (ipv4 eq '{ipAddress}')").ToArray();

            string prevSerialized = httpContext.Session.GetString("_mapsSession");
            if (string.IsNullOrWhiteSpace(prevSerialized))
            {
                return null;
            }

            return JsonSerializer.Deserialize<SessionLog>(prevSerialized);

            // find the latest one
            //SessionLog latest = sessions[0];
            //foreach(SessionLog session in sessions)
            //{
            //    if (session.SessionStart > latest.SessionStart)
            //    {
            //        latest = session;
            //    }
            //}

            //return latest;
        }


        /// <summary>
        /// Gets an initialized reference to a table data source
        /// </summary>
        /// <typeparam name="T">Type of business object to get table for</typeparam>
        /// <returns>AzureTablesDataSource ready to use</returns>
        private static AzureTablesDataSource GetDataSource<T>()
            where T : class
            => tableDataSources.GetOrAdd(
                    AzureTablesDataSource.GetTableName<T>(),
                    (tableName) =>
                    {
                        return new AzureTablesDataSource(tableConnectionString, tableName);
                    }
                );

        static CosmosClient()
        {
            tableConnectionString = AppSettingsJson.Configuration.GetSection("ConnectionStrings")["sessionLogDB"];
        }
        private static readonly string tableConnectionString;
        private static readonly ConcurrentDictionary<string, AzureTablesDataSource> tableDataSources = new ConcurrentDictionary<string, AzureTablesDataSource>();
    }
}
