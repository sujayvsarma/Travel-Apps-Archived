using Microsoft.ApplicationInsights.AspNetCore.TelemetryInitializers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OData.UriParser;
using Newtonsoft.Json;

using SujaySarma.Sdk.AspNetCore.Mvc;
using SujaySarma.Sdk.WikipediaApi.Pages;
using SujaySarma.Sdk.WikipediaApi.SerializationObjects;

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

using TravelIdeasPortalWeb.Objects;
using TravelIdeasPortalWeb.Objects.DB;
using TravelIdeasPortalWeb.Objects.RestCountries;
using TravelIdeasPortalWeb.Objects.Skyscanner;
using TravelIdeasPortalWeb.Objects.TIPW;
using TravelIdeasPortalWeb.Objects.TPC;
using TravelIdeasPortalWeb.ServiceClients;

namespace TravelIdeasPortalWeb.Controllers
{
    // handles the planning of tours
    [Controller]
    public class TourPlannerController : Controller
    {
        /// <summary>
        /// The initial endpoint at "/"
        /// </summary>
        [HttpGet("/")]
        public IActionResult Index()
        {
            GpsLocate();
            CreateAndBurySecretRequestToken();

            return View("MapUX");
        }

        /// <summary>
        /// Optimize and find tickets for the itinerary
        /// </summary>
        [HttpGet("/tickets")]
        public IActionResult OptimizeAndTicketItinerary([FromQuery(Name = "legs")] int numberOfEntries)
        {
            // Step 1: Decrypt the itinerary from the passed-in query string
            List<JSTripLeg> queryLegs = new List<JSTripLeg>();
            for (int i = 0; i < numberOfEntries; i++)
            {
                string qKey = $"leg{i}";
                if (Request.Query.ContainsKey(qKey))
                {
                    // "DEL|AMD|2019-11-09T00:00:00|1,0,0|economy"
                    string qValue = Request.Query[qKey];

                    if (!string.IsNullOrWhiteSpace(qValue))
                    {
                        string[] encodedLeg = qValue.Split(new char[] { '|' }, StringSplitOptions.None);
                        if (((encodedLeg.Length == 4) || (encodedLeg.Length == 5)) && (!string.IsNullOrWhiteSpace(encodedLeg[0])) && (!string.IsNullOrWhiteSpace(encodedLeg[1]))
                             && (!string.IsNullOrWhiteSpace(encodedLeg[2])) && (!string.IsNullOrWhiteSpace(encodedLeg[3]))
                             && DateTime.TryParse(encodedLeg[2], out DateTime date))
                        {
                            string[] origin = encodedLeg[0].Split(new char[] { ',' }, StringSplitOptions.None);
                            string[] destination = encodedLeg[1].Split(new char[] { ',' }, StringSplitOptions.None);
                            string[] pax = encodedLeg[3].Split(new char[] { ',' }, StringSplitOptions.None);
                            if ((origin.Length == 2) && (destination.Length == 2) && (pax.Length == 3))
                            {
                                string classOfTravel = (
                                        (encodedLeg.Length == 5)
                                        ? (string.IsNullOrWhiteSpace(encodedLeg[4]) ? "economy" : encodedLeg[4])
                                        : "economy"
                                    );

                                queryLegs.Add(
                                        new JSTripLeg()
                                        {
                                            Id = i,
                                            Origin = origin[0],
                                            OriginCountry = origin[1],
                                            Destination = destination[0],
                                            DestinationCountry = destination[1],
                                            Departure = date,
                                            Adults = ((pax[0].Length != 0) ? Convert.ToInt32(pax[0]) : 0),
                                            Children = ((pax[1].Length != 0) ? Convert.ToInt32(pax[1]) : 0),
                                            Infants = ((pax[2].Length != 0) ? Convert.ToInt32(pax[2]) : 0),
                                            ClassOfTravel = classOfTravel
                                        }
                                    );
                            }
                        }
                    }
                }
            }

            if (queryLegs.Count == 0)
            {
                return new RedirectResult("/", false);
            }

            string browserLocale = "en-US";
            if (Request.Query.ContainsKey("locale"))
            {
                browserLocale = Request.Query["locale"];
            }

            GpsLocate();

            string userCurrency = HttpContext.Session.GetString("__gpsCurrency") ?? "INR";
            string userCountry = HttpContext.Session.GetString("__gpsCountry") ?? "IN";

            // Step 2: Parse the decrypted itinerary, create the pricing requests while optimizing it along the way.
            Dictionary<int, SkyscannerPricingRequest> pricingRequests = new Dictionary<int, SkyscannerPricingRequest>();
            foreach (JSTripLeg tl in queryLegs)
            {
                bool wasMerged = false;
                foreach (SkyscannerPricingRequest pr in pricingRequests.Values)
                {
                    // are we a reverse leg of something already done?
                    // Origin<->Destination & PAX-counts have to match.
                    if ((pr.Origin.Iata == tl.Destination) && (pr.Destination.Iata == tl.Origin)
                            && (pr.Adults == tl.Adults) && (pr.Children == tl.Children) && (pr.Infants == tl.Infants)

                                // User may add the same leg multiple times, so ensure our return date is NULL
                                && (!pr.ReturnDeparture.HasValue))
                    {
                        pr.ReturnDeparture = tl.Departure;
                        wasMerged = true;
                        break;
                    }
                }

                if (!wasMerged)
                {
                    SkyCabinClass cabinClass = tl.ClassOfTravel switch
                    {
                        "premiumeconomy" => SkyCabinClass.PremiumEconomy,
                        "business" => SkyCabinClass.Business,
                        "first" => SkyCabinClass.First,
                        _ => SkyCabinClass.Economy
                    };

                    pricingRequests.Add(
                            tl.Id,
                            new SkyscannerPricingRequest()
                            {
                                Origin = new SkyscannerPricingRequestHalt()
                                {
                                    Iata = tl.Origin,
                                    IsoCountryCode = tl.OriginCountry,
                                    SkyId = SkyscannerClient.GetSkyscannerPlaceId(tl.Origin, tl.OriginCountry)
                                },
                                Destination = new SkyscannerPricingRequestHalt()
                                {
                                    Iata = tl.Destination,
                                    IsoCountryCode = tl.DestinationCountry,
                                    SkyId = SkyscannerClient.GetSkyscannerPlaceId(tl.Destination, tl.DestinationCountry)
                                },
                                OutboundDeparture = tl.Departure,
                                ReturnDeparture = null,
                                CabinClass = cabinClass,
                                Adults = tl.Adults,
                                Children = tl.Children,
                                Infants = tl.Infants,

                                UserLocale = browserLocale,
                                MarketCountryISO = userCountry,
                                UserCurrencyISO = userCurrency
                            }
                        );
                }
            }

            int ticketingAttempt = 0;

        tryGetTickets:

            ticketingAttempt++;

            // Step 3: Call Skyscanner API to get ticket prices
            //// We spin up multi-threaded parallel lookups to speed up our process
            ConcurrentDictionary<int, List<PricedItineraryOption>> pricingResults = new ConcurrentDictionary<int, List<PricedItineraryOption>>();
            Parallel.ForEach(
                    pricingRequests.Keys,
                    (id) => pricingResults.TryAdd(id, SkyscannerClient.GetTicketableQuotes(pricingRequests[id]))
                );

            // switch to a normal dictionary
            bool hasEmptyOptions = false;
            Dictionary<SkyscannerPricingRequest, List<PricedItineraryOption>> results = new Dictionary<SkyscannerPricingRequest, List<PricedItineraryOption>>();
            foreach (int id in pricingResults.Keys)
            {
                if ((pricingResults[id] == null) || (pricingResults[id].Count == 0))
                {
                    hasEmptyOptions = true;
                    break;
                }
                results.Add(pricingRequests[id], pricingResults[id]);
            }

            if (hasEmptyOptions)
            {
                if (ticketingAttempt == 3)
                {
                    // exhausted retries
                    results = null;
                }
                else
                {
                    goto tryGetTickets;
                }
            }

            // we're using Newtonsoft serialization here because the .NET Core 3.1 JsonSerializer does not *YET* 
            // support complex nested dictionaries. Argh!

            string tripId = HttpContext.Session.GetString("__uniqueId");
            CosmosClient.UpdateSession(
                    HttpContext,
                    tripId,
                    JsonConvert.SerializeObject(queryLegs, Newtonsoft.Json.Formatting.None),
                    JsonConvert.SerializeObject(pricingRequests, Newtonsoft.Json.Formatting.None),
                    JsonConvert.SerializeObject(new Dictionary<int, List<PricedItineraryOption>>(pricingResults), Newtonsoft.Json.Formatting.None)
                );

            // We dont want the user to hit refresh on the ticket-display UX, 
            // so we play a small trick and take them to the saved-trip UX instead
            return new RedirectResult($"/get/{tripId}");
        }

        /// <summary>
        /// Get a previously created trip with ticketing results, show the same ticket options again
        /// </summary>
        [HttpGet("/get/{uniqueId?}")]
        public IActionResult GetTrip([FromRoute(Name = "uniqueId")] string uniqueId)
        {
            uniqueId = uniqueId?.Trim();
            if (string.IsNullOrWhiteSpace(uniqueId))
            {
                return View("EnterTripId", null);
            }

            uniqueId = uniqueId.ToUpper();
            SessionLog log = CosmosClient.GetSession(HttpContext, uniqueId);
            if (log == null)
            {
                return View("EnterTripId", "ERROR");
            }

            // we're using Newtonsoft serialization here because the .NET Core 3.1 JsonSerializer does not *YET* 
            // support complex nested dictionaries. Argh!

            // reconstruct the viewable data
            Dictionary<int, SkyscannerPricingRequest> pricingRequests = JsonConvert.DeserializeObject<Dictionary<int, SkyscannerPricingRequest>>(log.OptimizedItinerary);
            Dictionary<int, List<PricedItineraryOption>> idResults = JsonConvert.DeserializeObject<Dictionary<int, List<PricedItineraryOption>>>(log.TicketResultJson);

            bool hasEmptyOptions = false;
            Dictionary<SkyscannerPricingRequest, List<PricedItineraryOption>> results = new Dictionary<SkyscannerPricingRequest, List<PricedItineraryOption>>();
            foreach (int id in idResults.Keys)
            {
                if ((idResults[id] == null) || (idResults[id].Count == 0))
                {
                    hasEmptyOptions = true;
                    break;
                }
                results.Add(pricingRequests[id], idResults[id]);
            }

            if (hasEmptyOptions)
            {
                results = null;
            }

            CreateAndBurySecretRequestToken();
            ViewData["__tripCurrencySymbol"] = log.GeoCurrencySymbol;
            ViewData["__uniqueId"] = uniqueId;

            return View("TicketingUX", results);
        }

        /// <summary>
        /// Track the outbound action and redirect user to the ticketing agent to make reservations
        /// </summary>
        [HttpGet("redirectTo/{token?}")]
        public IActionResult RedirectToTicketingAgent([FromRoute(Name = "token")] string requestToken, [FromQuery(Name = "tip")] string uniqueId, [FromQuery(Name = "locator")] string optionId = null)
        {
            if (string.IsNullOrWhiteSpace(optionId) || (!VerifySecretRequestToken(requestToken)))
            {
                return new NotFoundResult();
            }

            SessionLog log = CosmosClient.GetSession(HttpContext, uniqueId);
            if ((log == null) || string.IsNullOrWhiteSpace(log.TicketResultJson))
            {
                return new NotFoundResult();
            }

            string outboundURL = null;
            Dictionary<int, List<PricedItineraryOption>> idResults = JsonConvert.DeserializeObject<Dictionary<int, List<PricedItineraryOption>>>(log.TicketResultJson);
            bool pioFound = false;

            foreach (List<PricedItineraryOption> pioList in idResults.Values)
            {
                foreach (PricedItineraryOption pio in pioList)
                {
                    if (pio.Id.ToString("n") == optionId)
                    {
                        CosmosClient.TrackOutbound(uniqueId, pio.ForwardSegments[0].Origin.IATA, pio.ForwardSegments[pio.ForwardSegments.Count - 1].Destination.IATA,
                            HttpContext.Session.GetString("__gpsCurrencySymbol"), pio.Price, pio.Agent.Key);

                        outboundURL = pio.TicketingUri;
                        pioFound = true;
                        break;
                    }

                    if (pioFound)
                    {
                        break;
                    }
                }

                if (pioFound)
                {
                    break;
                }
            }

            if (!pioFound)
            {
                return new NotFoundResult();
            }

            return new RedirectResult(outboundURL, false);
        }

        ///////////////// Methods called from Ajax /////////////////


        /// <summary>
        /// Find out what we can about the location that was clicked
        /// </summary>
        /// <param name="requestToken">Secret request token</param>
        /// <param name="latitude">Latitude coordinates</param>
        /// <param name="longitude">Longitude coordinates</param>
        /// <returns>Json result</returns>
        [HttpGet("/ResolveLocation/{token?}")]
        public IActionResult GetLocationInformation([FromRoute(Name = "token")] string requestToken, [FromQuery(Name = "lat")] double latitude, [FromQuery(Name = "lon")] double longitude)
        {
            if (!VerifySecretRequestToken(requestToken))
            {
                return new OkObjectResult(string.Empty);
            }

            Objects.Coordinate queryLocation = new Coordinate(latitude, longitude);

            // reverse-lookup using Azure Maps API
            Objects.AzureMaps.ReverseGeoResult mapsReverseGeoResult = AzureMapsApiClient.GetLocationInfo(latitude, longitude);
            if ((mapsReverseGeoResult == null) || (mapsReverseGeoResult.Addresses.Count == 0))
            {
                return new OkObjectResult(string.Empty);
            }

            Objects.AzureMaps.ReverseGeoAddressItem addressResult = mapsReverseGeoResult.Addresses[0].Address;
            if (string.IsNullOrWhiteSpace(addressResult.City) || string.IsNullOrWhiteSpace(addressResult.Country))
            {
                return new OkObjectResult(string.Empty);
            }

            // Below this we will always return some meaningful result
            MapLocationInfo resultInfo = new MapLocationInfo()
            {
                Location = new Coordinate(latitude, longitude),
                City = addressResult.City,
                StateOrProvince = addressResult.StateOrProvince,
                CountryInfo = MapLocationCountryInfo.From(RestCountriesClient.GetCountry(addressResult.IsoCountry))
                                ?? new MapLocationCountryInfo()
                                {
                                    IsoCountryCode = addressResult.IsoCountry,
                                    Name = addressResult.Country,
                                    NativeName = addressResult.Country,
                                    Region = addressResult.StateOrProvince,
                                    Flag = $"https://www.countryflags.io/{addressResult.IsoCountry}/flat/64.png",

                                    // fill out stuff so that things dont crash and burn later
                                    BorderCountries = new List<string>() { },
                                    CapitalCity = "Unknown",
                                    Demonymn = addressResult.Country + "ian",
                                    Languages = new List<MapLocationCountryLanguage>() { new MapLocationCountryLanguage() { Name = "English", NativeName = "English" } },
                                    Subregion = "Not applicable",
                                    Timezones = new List<string>() { "Unknown" },
                                    Currencies = new List<MapLocationCountryCurrency>() { new MapLocationCountryCurrency() { Code = "USD", Name = "US Dollars", Symbol = "$" } }
                                },

                WikiPages = new List<MapLocationInfoWikiPage>(),
                Airports = new List<MapLocationAirportInfo>()
            };

            // Check Wikipedia
            PageClient pageClient = new PageClient();
            List<PageMetadata> wikipediaPages = pageClient.GetPagesRelatedTo(
                    addressResult.City?.Replace(" ", "_")
                    ?? addressResult.StateOrProvince?.Replace(" ", "_")
                    ?? addressResult.Country.Replace(" ", "_")
                );

            if (wikipediaPages.Count > 0)
            {
                foreach (PageMetadata pageM in wikipediaPages)
                {
                    if (!string.IsNullOrWhiteSpace(pageM.ExtractHtml))
                    {
                        resultInfo.WikiPages.Add(
                                new MapLocationInfoWikiPage()
                                {
                                    DisplayTitle = pageM.DisplayTitle,
                                    ThumbnailImage = pageM.Thumbnail?.ImageURI,
                                    WikipageURL = pageM.ContentURI["desktop"]["page"],
                                    InfoTextHtml = pageM.ExtractHtml
                                }
                            );
                    }
                }
            }

            // Check outbound flights from the location
            List<Objects.OurAirport.OurAirportInfoResult> nearestAirports = OurAirportsApiClient.GetNearestAirports(latitude, longitude, 5);
            foreach (Objects.OurAirport.OurAirportInfoResult airportInfoResult in nearestAirports)
            {
                resultInfo.Airports.Add(
                        new MapLocationAirportInfo()
                        {
                            GPS = airportInfoResult.Airport.GPS,
                            HomeUri = airportInfoResult.Airport.HomeUri,
                            Iata = airportInfoResult.Airport.Iata,
                            Municipality = airportInfoResult.Airport.Municipality,
                            Name = airportInfoResult.Airport.Name,
                            WikipediaUri = airportInfoResult.Airport.WikipediaUri,
                            Latitude = airportInfoResult.Airport.Latitude,
                            Longitude = airportInfoResult.Airport.Longitude,
                            DistanceFromLocation = Math.Round(queryLocation.DistanceTo(new Coordinate(airportInfoResult.Airport.Latitude, airportInfoResult.Airport.Longitude)), 0),

                            CountryName = airportInfoResult.Country.Name,
                            ISOCountryCode = airportInfoResult.Country.Code,
                            State = airportInfoResult.StateOrProvince.Name
                        }
                    );
            }


            return new JsonResult(
                    resultInfo,
                    new JsonSerializerOptions()
                    {
                        IgnoreNullValues = false,
                        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                        WriteIndented = false
                    }
                );
        }


        /// <summary>
        /// Helper function that provides the Azure Maps Control with a bearer token for authentication with Azure. 
        /// This function is called via an Ajax function from ~/Resources/js/fly2-tourplanner.js
        /// </summary>
        /// <param name="requestToken">The session Xsrf token to prevent token hijacking</param>
        /// <returns>The access token</returns>
        [HttpGet("/MapsAuthenticate/{token?}")]
        public IActionResult GetAzureMapsAccessToken([FromRoute(Name = "token")] string requestToken)
        {
            if (!VerifySecretRequestToken(requestToken))
            {
                return new OkObjectResult(string.Empty);
            }

            string token = AzureMapsApiClient.GetBearerToken();
            return new OkObjectResult(token);
        }



        ///////////////// PRIVATE FUNCTIONS BELOW THIS LINE /////////////////


        private void GpsLocate()
        {
            string ipAddress = HttpContext.Connection.RemoteIpAddress.ToString();
            if ((ipAddress == "::1") || (AppSettingsJson.Environment.IsDevelopment()))
            {
                ipAddress = "103.211.52.187";   // a delhi IP address
            }

            string sessionUniqueId = HttpContext.Session.GetString("__uniqueId");
            SessionLog session = ((sessionUniqueId == null) ? CosmosClient.TryGetExistingSession(HttpContext, ipAddress) : CosmosClient.GetSession(HttpContext, sessionUniqueId));
            if (session == null)
            {
                // honor rate limiting
                if ((_ipLocation_rateLimit_remaining <= 0) && (_ipLocation_rateLimit_secondsToReset > 0))
                {
                    Thread.Sleep((_ipLocation_rateLimit_secondsToReset + 5) * 1000);
                }

                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, $"http://ip-api.com/json/{ipAddress}?fields=country,countryCode,lat,lon,currency");
                request.Headers.Add("Accept", "application/json");

                HttpClient client = new HttpClient();
                HttpResponseMessage responseMessage = client.SendAsync(request).Result;

                _ipLocation_rateLimit_secondsToReset = Convert.ToInt32(responseMessage.Headers.GetValues("X-Ttl").ToArray()[0]);
                _ipLocation_rateLimit_remaining = Convert.ToInt32(responseMessage.Headers.GetValues("X-Rl").ToArray()[0]);

                if (responseMessage.IsSuccessStatusCode)
                {
                    Dictionary<string, object> response = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, object>>(responseMessage.Content.ReadAsStringAsync().Result);

                    string currencyIso = ((JsonElement)response["currency"]).GetString();

                    sessionUniqueId = CosmosClient.CreateNewSession(
                        HttpContext,
                        HttpContext.Session.Id, ipAddress,
                        new Coordinate()
                        {
                            Latitude = ((JsonElement)response["lat"]).GetDouble(),
                            Longitude = ((JsonElement)response["lon"]).GetDouble()
                        },
                        new RestCountry()
                        {
                            Name = ((JsonElement)response["country"]).GetString(),
                            IsoCountryCode = ((JsonElement)response["countryCode"]).GetString(),
                            Currencies = new List<RestCountryCurrency>() { new RestCountryCurrency() { Code = currencyIso, Symbol = currencyIso } }
                        }
                    );

                    session = CosmosClient.GetSession(HttpContext, sessionUniqueId);
                }                
            }

            ViewData["lat"] = session.GeoLocLat;
            ViewData["lon"] = session.GeoLocLon;

            HttpContext.Session.SetString("__gpsCountry", session.GeoCountryISO);
            HttpContext.Session.SetString("__gpsCurrency", session.GeoCurrencyName);
            HttpContext.Session.SetString("__gpsCurrencySymbol", session.GeoCurrencySymbol);
            HttpContext.Session.SetString("__uniqueId", session.RowKey);
        }
        private static int _ipLocation_rateLimit_remaining = 45;
        private static int _ipLocation_rateLimit_secondsToReset = 0;


        /// <summary>
        /// Create the anti request forgery token and buries it in the session
        /// </summary>
        private void CreateAndBurySecretRequestToken()
        {
            HttpContext.Session.SetString("_XsrfToken", Guid.NewGuid().ToString("n"));
        }

        /// <summary>
        /// Verify the token in the request as being the one we originally generated
        /// </summary>
        /// <param name="token">Token from the request</param>
        /// <returns>True if the tokens match</returns>
        private bool VerifySecretRequestToken(string token)
        {
            string actualToken = HttpContext.Session.GetString("_XsrfToken");
            return ((!string.IsNullOrWhiteSpace(token)) && (!string.IsNullOrWhiteSpace(actualToken)) && (token == actualToken));
        }
    }
}
