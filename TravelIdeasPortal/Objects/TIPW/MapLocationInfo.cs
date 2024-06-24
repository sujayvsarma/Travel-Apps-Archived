using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace TravelIdeasPortalWeb.Objects.TIPW
{
    /// <summary>
    /// Combined collated information about a location the user clicked on the map UX
    /// </summary>
    public class MapLocationInfo
    {
        /// <summary>
        /// Coordinate of the location
        /// </summary>
        [JsonPropertyName("latLon")]
        public Coordinate Location { get; set; }

        /// <summary>
        /// Information about the country
        /// </summary>
        [JsonPropertyName("country")]
        public MapLocationCountryInfo CountryInfo { get; set; }

        /// <summary>
        /// The state or province name
        /// </summary>
        [JsonPropertyName("state")]
        public string StateOrProvince { get; set; }

        /// <summary>
        /// The city limits name
        /// </summary>
        [JsonPropertyName("city")]
        public string City { get; set; }

        /// <summary>
        /// Wikipedia links for the pages
        /// </summary>
        [JsonPropertyName("wikiLinks")]
        public List<MapLocationInfoWikiPage> WikiPages { get; set; }

        /// <summary>
        /// Airports nearest to this location
        /// </summary>
        [JsonPropertyName("airports")]
        public List<MapLocationAirportInfo> Airports { get; set; }

    }
}
