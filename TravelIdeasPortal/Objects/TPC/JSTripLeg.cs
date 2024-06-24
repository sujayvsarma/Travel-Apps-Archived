using System;

namespace TravelIdeasPortalWeb.Objects.TPC
{
    /// <summary>
    /// The TripLeg object from the mapux javascript 
    /// (defined in: /staticcontent/scripts/mapux.js as "TripLeg")
    /// </summary>
    public class JSTripLeg
    {
        /// <summary>
        /// Numeric serial number
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// IATA
        /// </summary>
        public string Origin { get; set; }

        /// <summary>
        /// ISO Country code of Origin
        /// </summary>
        public string OriginCountry { get; set; }

        /// <summary>
        /// IATA
        /// </summary>
        public string Destination { get; set; }

        /// <summary>
        /// ISO Country code of Destination
        /// </summary>
        public string DestinationCountry { get; set; }

        /// <summary>
        /// Date of departure
        /// </summary>
        public DateTime Departure { get; set; }

        /// <summary>
        /// Number of adult passengers
        /// </summary>
        public int Adults { get; set; }

        /// <summary>
        /// Number of child passengers
        /// </summary>
        public int Children { get; set; }

        /// <summary>
        /// Number of infant passengers
        /// </summary>
        public int Infants { get; set; }

        /// <summary>
        /// Class of travel ("economy","premiumeconomy","business","first")
        /// </summary>
        public string ClassOfTravel { get; set; }
    }
}
