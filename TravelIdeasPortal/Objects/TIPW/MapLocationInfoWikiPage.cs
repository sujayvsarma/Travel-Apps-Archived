using System.Text.Json.Serialization;

namespace TravelIdeasPortalWeb.Objects.TIPW
{
    /// <summary>
    /// Wikipedia page about a location on a map
    /// </summary>
    public class MapLocationInfoWikiPage
    {
        /// <summary>
        /// Display title for the page
        /// </summary>
        [JsonPropertyName("title")]
        public string DisplayTitle { get; set; }

        /// <summary>
        /// Thumbnail image if it exists
        /// </summary>
        [JsonPropertyName("thumbnail")]
        public string ThumbnailImage { get; set; }

        /// <summary>
        /// URL to the wikipedia page
        /// </summary>
        [JsonPropertyName("wikiPageUrl")]
        public string WikipageURL { get; set; }

        /// <summary>
        /// Summary text (Html) about the place
        /// </summary>
        [JsonPropertyName("text")]
        public string InfoTextHtml { get; set; }
    }
}
