using SujaySarma.Web.TravelWeb.Data.Airports;
using SujaySarma.Web.TravelWeb.Data.Countries;
using SujaySarma.Web.TravelWeb.Data.Wiki;

using System.Text.Json.Serialization;

namespace SujaySarma.Web.TravelWeb.Data
{
    /// <summary>
    /// Collated information about a location on the map
    /// </summary>
    public class MapLocationInfo
    {

        /// <summary>
        /// Coordinate of the location
        /// </summary>
        [JsonPropertyName("latLon")]
        public Coordinate? Location { get; set; }

        /// <summary>
        /// Country
        /// </summary>
        [JsonPropertyName("country")]
        public Country? Country { get; set; }

        /// <summary>
        /// City
        /// </summary>
        [JsonPropertyName("city")]
        public string? City { get; set; }

        /// <summary>
        /// State or province
        /// </summary>
        [JsonPropertyName("state")]
        public string? State { get; set; }

        /// <summary>
        /// Wikipedia links for the pages
        /// </summary>
        [JsonPropertyName("wikiLinks")]
        public List<LocationInfoWikiPage>? WikiPages { get; set; }

        /// <summary>
        /// Airports nearest to this location
        /// </summary>
        [JsonPropertyName("airports")]
        public List<Airport>? Airports { get; set; }

    }

}
