using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace SujaySarma.WebAPI.AirportFinder.Internal
{
    /// <summary>
    /// The structure used to standardize Json responses to queries
    /// </summary>
    internal class JsonResultsConstruct<T>
    {
        /// <summary>
        /// Number of results
        /// </summary>
        [JsonPropertyName("count")]
        public int Count { get; set; } = 0;

        /// <summary>
        /// Results of query
        /// </summary>
        [JsonPropertyName("results")]
        public IEnumerable<T>? ResultObject { get; set; } = default;

        /// <summary>
        /// Return a result object
        /// </summary>
        /// <param name="results">The results data</param>
        /// <returns>Result object</returns>
        public static JsonResultsConstruct<T> GetJsonResult(IEnumerable<T> results)
            => new()
            {
                Count = ((results == null) ? 0 : results.Count()),
                ResultObject = results
            };
    }
}
