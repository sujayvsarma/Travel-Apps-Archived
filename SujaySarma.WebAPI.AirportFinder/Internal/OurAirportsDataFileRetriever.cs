using System;
using System.IO;
using System.Linq;
using System.Net.Http;

namespace SujaySarma.WebAPI.AirportFinder.Internal
{
    /// <summary>
    /// Contains functionality to check for and retrieve the latest file 
    /// from the OurAirports data source
    /// </summary>
    internal static class OurAirportsDataFileRetriever
    {

        /// <summary>
        /// Ensure the lastest file on local cache
        /// </summary>
        /// <param name="file">The file to fetch</param>
        /// <param name="environmentContentRoot">ContentRoot path from WebHostEnvironment</param>
        /// <returns></returns>
        public static bool EnsureLatestFile(Constants.DataFileType file, string environmentContentRoot)
        {
            string fileName = file switch
            {
                Constants.DataFileType.airports => "airports",
                Constants.DataFileType.countries => "countries",
                Constants.DataFileType.regions => "regions",
                _ => throw new ArgumentException("Value of 'file' out of range", nameof(file))
            };

            string localPath = Path.Combine(environmentContentRoot, "_data", $"{fileName}.csv");
            Uri repoUrl = new ($"{BASE_URL}/{fileName}.csv");

            DateTime fileUpdated = ((File.Exists(localPath)) ? (new FileInfo(localPath)).LastWriteTimeUtc : DateTime.MinValue);
            if (fileUpdated > fileUpdateThreshold)
            {
                return true;
            }

            HttpRequestMessage requestMessage = new(HttpMethod.Head, repoUrl);
            HttpResponseMessage responseMessage = client.Send(requestMessage);
            string? lastModifiedString = responseMessage.Content.Headers.GetValues("Last-Modified").FirstOrDefault();
            if (string.IsNullOrWhiteSpace(lastModifiedString))
            {
                return false;
            }

            DateTime serverUpdated = DateTime.Parse(lastModifiedString);
            if (serverUpdated < fileUpdateThreshold)
            {
                return true;
            }

            requestMessage = new HttpRequestMessage(HttpMethod.Get, repoUrl);
            responseMessage = client.Send(requestMessage);
            if (responseMessage.IsSuccessStatusCode && responseMessage.Content.Headers.ContentLength.HasValue && (responseMessage.Content.Headers.ContentLength.Value > 0))
            {
                if (fileUpdated < serverUpdated)
                {
                    File.Delete(localPath);
                }

                File.WriteAllText(localPath, responseMessage.Content.ReadAsStringAsync().Result);
                return true;
            }

            return false;
        }


        private static readonly HttpClient client = new HttpClient();
        private static readonly DateTime fileUpdateThreshold = DateTime.Today.AddDays(-30);
        private static readonly string BASE_URL = "https://davidmegginson.github.io/ourairports-data";

    }
}
