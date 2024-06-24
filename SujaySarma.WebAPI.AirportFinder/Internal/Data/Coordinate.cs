using System.Text.Json.Serialization;

namespace SujaySarma.Api.OurAirports.Business
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
        /// Initialize 
        /// </summary>
        /// <param name="lat">Latitude</param>
        /// <param name="lon">Longitude</param>
        public Coordinate(double lat, double lon)
        {
            Latitude = lat;
            Longitude = lon;
        }


    }
}
