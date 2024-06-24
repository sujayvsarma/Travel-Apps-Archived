using Microsoft.AspNetCore.Mvc;

using SujaySarma.WebAPI.AirportFinder.Internal;

using System;
using System.Collections.Generic;
using System.Linq;

namespace SujaySarma.WebAPI.AirportFinder
{
    /// <summary>
    /// Utilities class
    /// </summary>
    internal static class Utils
    {
        /// <summary>
        /// Get a Json result
        /// </summary>
        /// <typeparam name="T">Type of member object</typeparam>
        /// <param name="results">Collection of results</param>
        /// <returns>JsonResult</returns>
        public static JsonResult GetJson<T>(IEnumerable<T> results)
            => new JsonResult(
                new JsonResultsConstruct<T>()
                {
                    Count = results.Count(),
                    ResultObject = results.ToList()
                },
                Constants.serializerOptions
            );

        /// <summary>
        /// Assert that a value is not null or empty
        /// </summary>
        /// <param name="value">value to assert</param>
        public static void AssertNotEmpty(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentNullException();
            }
        }

    }
}
