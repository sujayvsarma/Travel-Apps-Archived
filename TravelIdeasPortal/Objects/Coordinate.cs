using System;
using System.Text.Json.Serialization;

namespace TravelIdeasPortalWeb.Objects
{
    /// <summary>
    /// Geographic coordinate
    /// </summary>
    public class Coordinate
    {
        /// <summary>
        /// Latitude
        /// </summary>
        [JsonPropertyName("lat")]
        public double Latitude { get; set; }

        /// <summary>
        /// Longitude
        /// </summary>
        [JsonPropertyName("lon")]
        public double Longitude { get; set; }

        /// <summary>
        /// Default constructor
        /// </summary>
        public Coordinate() { }

        /// <summary>
        /// Initialize 
        /// </summary>
        /// <param name="lat">Latitude</param>
        /// <param name="lon">Longitude</param>
        public Coordinate(double lat, double lon)
        {
            Latitude = lat;
            Longitude = lon;
        }


        /// <summary>
        /// Initialize
        /// </summary>
        /// <param name="coordinates">Coordinates in "lat,lon" format</param>
        public Coordinate(string coordinates)
        {
            if (coordinates.IndexOf(',') <= 0)
            {
                throw new ArgumentException(nameof(coordinates));
            }

            Latitude = Convert.ToDouble(coordinates[0..coordinates.IndexOf(',')]);
            Longitude = Convert.ToDouble(coordinates[(coordinates.IndexOf(',') + 1)..]);
        }

        /// <summary>
        /// Calculate distance between two geographic coordinates
        /// </summary>
        /// <param name="to">Destination point</param>
        /// <returns>Distance in KM</returns>
        public double DistanceTo(Coordinate to)
        {
            var baseRad = Math.PI * Latitude / 180;
            var targetRad = Math.PI * to.Latitude / 180;
            var theta = Longitude - to.Longitude;
            var thetaRad = Math.PI * theta / 180;

            double dist =
                Math.Sin(baseRad) * Math.Sin(targetRad) + Math.Cos(baseRad) *
                Math.Cos(targetRad) * Math.Cos(thetaRad);

            return Math.Acos(dist) * dist_calc_conv_factor;

        }
        // this is basically: ((180 * 60 * 1.1515 * 1.609344) / Math.PI) calculated using C# REPL
        private static readonly double dist_calc_conv_factor = 6370.6934856530579;
    }
}
