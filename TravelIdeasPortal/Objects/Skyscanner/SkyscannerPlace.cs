using System.Text.Json.Serialization;

namespace TravelIdeasPortalWeb.Objects.Skyscanner
{
    /// <summary>
    /// A place as returned by the Skyscanner autosuggest API
    /// </summary>
    public class SkyscannerPlace
    {

        /// <summary>
        /// The Skyscanner Place ID - this is what is to be used for this place in other API
        /// </summary>
        [JsonPropertyName("PlaceId")]
        public string SkyId { get; set; }

    }

}
