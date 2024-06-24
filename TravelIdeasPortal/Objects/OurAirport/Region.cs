using System.Text.Json.Serialization;

namespace TravelIdeasPortalWeb.Objects.OurAirport
{
    /// <summary>
    /// Denotes a region (OurAirports > regions.csv)
    /// </summary>
    public class Region
    {
        /// <summary>
        /// Name of the region
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; set; }

    }
}
