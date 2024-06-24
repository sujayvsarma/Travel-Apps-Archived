using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace TravelIdeasPortalWeb.Objects.Skyscanner
{
    /// <summary>
    /// The actual full result returned by the Skyscanner autosuggest API
    /// </summary>
    public class SkyscannerPlaceList
    {
        /// <summary>
        /// Items in the list
        /// </summary>
        [JsonPropertyName("Places")]
        public List<SkyscannerPlace> Items { get; set; }
    }
}
