using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SujaySarma.WebAPI.AirportFinder.Internal
{
    /// <summary>
    /// Class containing constants
    /// </summary>
    internal static class Constants
    {
        /// <summary>
        /// The type of data file to use
        /// </summary>
        public enum DataFileType
        {
            /// <summary>
            /// The airports file
            /// </summary>
            airports,

            /// <summary>
            /// The countries file
            /// </summary>
            countries,

            /// <summary>
            /// The regions file
            /// </summary>
            regions
        }

        /// <summary>
        /// Serialization options
        /// </summary>
        public static readonly JsonSerializerOptions serializerOptions = new JsonSerializerOptions()
        {
            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            AllowTrailingCommas = false,
            DefaultIgnoreCondition = JsonIgnoreCondition.Never,
            WriteIndented = false
        };

        public static readonly Dictionary<string, string> Continents = new()
                    {
                        { "AF" ,  "Africa" },
                        { "AN" ,  "Antarctica" },
                        { "AS" ,  "Asia" },
                        { "EU" ,  "Europe" },
                        { "NA" ,  "North America" },
                        { "OC" ,  "Oceanic" },
                        { "SA" ,  "South America" }
                    };

    }
}
