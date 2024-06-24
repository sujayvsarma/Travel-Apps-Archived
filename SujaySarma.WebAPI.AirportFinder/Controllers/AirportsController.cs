using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

using SujaySarma.WebAPI.AirportFinder.Internal;
using SujaySarma.WebAPI.AirportFinder.Internal.Data;

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;

namespace SujaySarma.WebAPI.AirportFinder.Controllers
{
    /// <summary>
    /// Search for and return information about airports
    /// </summary>
    [ApiController]
    public class AirportsController : ControllerBase
    {

        /// <summary>
        /// Get an airport by the numeric ID
        /// </summary>
        /// <param name="id">Numeric ID of the airport</param>
        /// <returns>Array of matching airports</returns>
        [HttpGet("api/airport/id/{id:int}")]
        public IActionResult GetById([FromRoute(Name = "id")] int id)
            => Utils.GetJson(_ds_airports.Items.Where(c => (c.Id == id)));

        /// <summary>
        /// Get a airport by the IATA code
        /// </summary>
        /// <param name="iata">IATA airport Code</param>
        /// <returns>Array of matching airports</returns>
        [HttpGet("api/airport/iata/{iata}")]
        public IActionResult GetByIATACode([FromRoute(Name = "iata")] string iata)
        {
            Utils.AssertNotEmpty(iata);
            return Utils.GetJson(_ds_airports.Items.Where(c => (c.Iata == iata)));
        }

        /// <summary>
        /// Get a airport by the ICAO code
        /// </summary>
        /// <param name="icao">ICAO airport Code</param>
        /// <returns>Array of matching airports</returns>
        [HttpGet("api/airport/icao/{icao}")]
        public IActionResult GetByICAOCode([FromRoute(Name = "icao")] string icao)
        {
            Utils.AssertNotEmpty(icao);
            return Utils.GetJson(_ds_airports.Items.Where(c => (c.Icao == icao)));
        }

        /// <summary>
        /// Get a airport by it's name
        /// </summary>
        /// <param name="name">Name of the airport</param>
        /// <returns>Array of matching airports</returns>
        [HttpGet("api/airport/name/{name}")]
        public IActionResult GetByName([FromRoute(Name = "name")] string name)
        {
            Utils.AssertNotEmpty(name);
            return Utils.GetJson(_ds_airports.Items.Where(
                c => (
                    c.Name!.Contains(name, StringComparison.InvariantCultureIgnoreCase)
                    || ((!string.IsNullOrWhiteSpace(c.Keywords)) && c.Keywords.Contains(name, StringComparison.InvariantCultureIgnoreCase))
                )
            ));
        }

        /// <summary>
        /// Get airports in the given country, state/province or city
        /// </summary>
        /// <param name="locality">2 letter ISO country code (eg: "IN") or 5 character ISO region/city code (eg: "IN-DL")</param>
        /// <returns>Array of matching airports</returns>
        [HttpGet("api/airports/{locality}")]
        public IActionResult GetByLocality([FromRoute(Name = "locality")] string locality)
        {
            Utils.AssertNotEmpty(locality);
            if (string.IsNullOrWhiteSpace(locality) || (locality.Length < 2) || (locality.Length > 5))
            {
                return base.BadRequest();
            }

            return Utils.GetJson(_ds_airports.Items.Where(
                c => (
                    ((!string.IsNullOrWhiteSpace(c.Country)) && c.Country.Contains(locality, StringComparison.InvariantCultureIgnoreCase))
                    || ((!string.IsNullOrWhiteSpace(c.State)) && c.State.Contains(locality, StringComparison.InvariantCultureIgnoreCase)) 
                    || ((!string.IsNullOrWhiteSpace(c.Municipality)) && c.Municipality.Contains(locality, StringComparison.InvariantCultureIgnoreCase))
                )
            ));
        }


        /// <summary>
        /// Find the airport nearest to provided Lat/Lon coordinates
        /// </summary>
        /// <param name="latitude">Latitude coordinate</param>
        /// <param name="longitude">Longitude coordinate</param>
        /// <param name="iataOnly">If set (default: true), shows only IATA (commercial) airports</param>
        /// <param name="maxResults">If set (default: 1), shows only a max of this number of results</param>
        /// <returns>Array of matching airports</returns>
        [HttpGet("api/airports/nearest/latlon")]
        public IActionResult FindNearestByLatLon([FromQuery(Name = "lat")] double latitude, [FromQuery(Name = "lon")] double longitude,
            [FromQuery(Name = "iataOnly")] bool iataOnly = true, [FromQuery(Name = "max")] int maxResults = 1)
            => GetAirportByDistances(new()
            {
                Latitude = latitude,
                Longitude = longitude
            }, iataOnly, maxResults);

        /// <summary>
        /// Find the airport nearest to provided IPv4 address
        /// </summary>
        /// <param name="ipAddress">IPv4 address</param>
        /// <param name="iataOnly">If set (default: true), shows only IATA (commercial) airports</param>
        /// <param name="maxResults">If set (default: 1), shows only a max of this number of results</param>
        /// <returns>Array of matching airports</returns>
        [HttpGet("api/airports/nearest/ip")]
        public IActionResult FindNearestByLatLon([FromQuery(Name = "ipAddress")] string ipAddress, 
            [FromQuery(Name = "iataOnly")] bool iataOnly = true, [FromQuery(Name = "max")] int maxResults = 1)
            => GetAirportByDistances(IP2Location(ipAddress), iataOnly, maxResults);



        private JsonResult GetAirportByDistances(Coordinate inputLocation, bool iataOnly, int maxResults)
        {
            Dictionary<double, Airport> airportsByDistance = new();
            foreach (Airport airport in _ds_airports.Items)
            {
                if (iataOnly && string.IsNullOrWhiteSpace(airport.Iata))
                {
                    continue;
                }

                Coordinate airport_location = new()
                {
                    Latitude = Convert.ToDouble(airport.Latitude),
                    Longitude = Convert.ToDouble(airport.Longitude)
                };

                double distance = GetDistance(inputLocation, airport_location);
                while (airportsByDistance.ContainsKey(distance))
                {
                    // its possible :-( Add an inconsequential distance to be able to add it
                    distance += 0.00000001;
                }

                airportsByDistance.Add(distance, airport);
            }

            List<AirportByDistance> results = new();
            if (airportsByDistance.Count > 0)
            {   
                foreach (double distanceKey in airportsByDistance.Keys.OrderBy(d => d).ThenBy(d => airportsByDistance[d].Iata).Take(maxResults))
                {
                    results.Add(
                            new AirportByDistance()
                            {
                                DistanceByKilometers = Convert.ToInt32(distanceKey),
                                Airport = airportsByDistance[distanceKey]
                            }
                        );
                }
            }

            return Utils.GetJson(results);
        }


        // calculate distance between two geographic coordinates
        private double GetDistance(Coordinate from, Coordinate to)
        {
            var baseRad = Math.PI * from.Latitude / 180;
            var targetRad = Math.PI * to.Latitude / 180;
            var theta = from.Longitude - to.Longitude;
            var thetaRad = Math.PI * theta / 180;

            double dist =
                Math.Sin(baseRad) * Math.Sin(targetRad) + Math.Cos(baseRad) *
                Math.Cos(targetRad) * Math.Cos(thetaRad);

            dist = Math.Acos(dist);

            dist = dist * 180 / Math.PI;
            dist = dist * 60 * 1.1515;

            return dist_to_km_factor * dist;
        }

        private Coordinate IP2Location(string ipAddress)
        {
            Coordinate resolution = _ipLocationCache.GetOrAdd(
                        ipAddress,
                        (ipAddr) =>
                        {
                                // honor rate limiting
                            if ((_ipLocation_rateLimit_remaining <= 0) && (_ipLocation_rateLimit_secondsToReset > 0))
                            {
                                Thread.Sleep((_ipLocation_rateLimit_secondsToReset + 5) * 1000);
                            }

                            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, $"http://ip-api.com/json/{ipAddr}?fields=lat,lon");
                            request.Headers.Add("Accept", "application/json");

                            HttpClient client = new();
                            HttpResponseMessage responseMessage = client.Send(request);

                            _ipLocation_rateLimit_secondsToReset = Convert.ToInt32(responseMessage.Headers.GetValues("X-Ttl").ToArray()[0]);
                            _ipLocation_rateLimit_remaining = Convert.ToInt32(responseMessage.Headers.GetValues("X-Rl").ToArray()[0]);

                            if (responseMessage.IsSuccessStatusCode)
                            {
                                Dictionary<string, double>? response = JsonSerializer.Deserialize<Dictionary<string, double>>(responseMessage.Content.ReadAsStringAsync().Result);
                                if (response != null)
                                {
                                    return new Coordinate()
                                    {
                                        Latitude = response["lat"],
                                        Longitude = response["lon"]
                                    };
                                }
                            }

                            return new Coordinate();
                        }
                    );

            if ((resolution.Latitude == 0) && (resolution.Longitude == 0))
            {
                // there was an error in the resolution,
                // invalidate the item, maybe next fetch can resolve it
                _ipLocationCache.Remove(ipAddress, out _);
            }

            return resolution;
        }


        /// <inheritdoc />
        public AirportsController(IWebHostEnvironment webHostEnvironment)
        {
            _ds_airports = new DataSource<Airport>(
                    Constants.DataFileType.airports, webHostEnvironment.ContentRootPath,
                    (t) =>
                    {
                        foreach (DataRow r in t.Rows)
                        {
                            string? ss = r["scheduled_service"] as string;
                            r["scheduled_service"] = ss?.Replace("no", "false").Replace("yes", "true");
                        }
                    }
                );
        }
        private static readonly double dist_to_km_factor = 1.609344d;
        private static readonly ConcurrentDictionary<string, Coordinate> _ipLocationCache = new ConcurrentDictionary<string, Coordinate>();
        private static int _ipLocation_rateLimit_remaining = 45;
        private static int _ipLocation_rateLimit_secondsToReset = 0;

        private readonly DataSource<Airport> _ds_airports;


        private class AirportByDistance
        {
            /// <summary>
            /// Distance in Kilometers
            /// </summary>
            [JsonPropertyName("dist_km")]
            public double DistanceByKilometers { get; set; }

            /// <summary>
            /// Airport
            /// </summary>
            [JsonPropertyName("airport")]
            public Airport Airport { get; set; } = default!;

        }

        private class Coordinate
        {
            /// <summary>
            /// Latitude
            /// </summary>
            public double Latitude { get; set; } = 0.0;

            /// <summary>
            /// Longitude
            /// </summary>
            public double Longitude { get; set; } = 0.0;

        }
    }
}
