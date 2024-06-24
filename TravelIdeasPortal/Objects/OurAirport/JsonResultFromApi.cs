using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace TravelIdeasPortalWeb.Objects.OurAirport
{
    /// <summary>
    /// The structure used to standardize Json responses to queries
    /// </summary>
    internal class JsonResultFromApi<T>
    {
        /// <summary>
        /// If true, caller must check errorMessage for the text
        /// </summary>
        [JsonPropertyName("isError")]
        public bool IsError { get; set; } = false;

        /// <summary>
        /// Number of results
        /// </summary>
        [JsonPropertyName("count")]
        public int Count { get; set; } = 0;

        /// <summary>
        /// Results of query
        /// </summary>
        [JsonPropertyName("results")]
        public IEnumerable<T> ResultObject { get; set; } = default;

        /// <summary>
        /// If methods are passed the argument to fully resolve data, the 
        /// connect-up data will be populated here
        /// </summary>
        [JsonPropertyName("lookup")]
        public Dictionary<string, object> ExtensionResults { get; set; } = null;
    }
}
