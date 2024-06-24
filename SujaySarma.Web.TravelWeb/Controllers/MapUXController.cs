using Microsoft.AspNetCore.Mvc;

using SujaySarma.Sdk.WikipediaApi.Pages;
using SujaySarma.Sdk.WikipediaApi.SerializationObjects;
using SujaySarma.Web.TravelWeb.ApiClients;
using SujaySarma.Web.TravelWeb.Data;
using SujaySarma.Web.TravelWeb.Data.Airports;
using SujaySarma.Web.TravelWeb.Data.AzureMaps;
using SujaySarma.Web.TravelWeb.Data.Wiki;

using System.Text.Encodings.Web;
using System.Text.Json;

namespace SujaySarma.Web.TravelWeb.Controllers
{
    public class MapUXController : Controller
    {
        [HttpGet("/")]
        public IActionResult Index()
        {
            ViewData["RequestHostName"] = Request.Host.ToString();
            ViewData["MapsAuthKey"] = configuration.GetSection("azureMaps")["AuthenticationPrimaryKey"];

            GpsLocate();
            GenerateXsrf();            
            return View("MapPlanner");
        }


        [HttpGet("/getinfo/{xsrfToken}")]
        public IActionResult GetLocationInformation([FromRoute(Name = "xsrfToken")] string? xsrfToken, [FromQuery(Name = "lat")] double lat, [FromQuery(Name = "lon")] double lon)
        {
            if (string.IsNullOrEmpty(xsrfToken))
            {
                return base.Forbid();
            }

            Coordinate loc = new(lat, lon);
            ReverseGeoAddressItem? address = AzureMapsApiClient.ReverseGeoLookup(configuration.GetSection("azureMaps")["AuthenticationPrimaryKey"], lat, lon);
            if ((address == null) || string.IsNullOrWhiteSpace(address.IsoCountry))
            {
                return base.NotFound();
            }

            Data.Countries.Country? country = CountriesApiClient.GetCountry(address.IsoCountry);
            List<Data.Airports.Airport>? airports = AirportFinderApiClient.GetNearest(lat, lon, 5);

            MapLocationInfo locationInfo = new()
            {
                Location = loc,
                Country = country,
                State = address.StateOrProvince,
                City = address.City,
                Airports = airports,
                WikiPages = new()
            };

            // Check Wikipedia
            PageClient pageClient = new ();
            List<PageMetadata> wikipediaPages = pageClient.GetPagesRelatedTo(
                    address.City?.Replace(" ", "_") ?? address.StateOrProvince?.Replace(" ", "_") ?? address.Country!.Replace(" ", "_")
                );

            if (wikipediaPages.Count > 0)
            {
                foreach (PageMetadata pageM in wikipediaPages)
                {
                    if (!string.IsNullOrWhiteSpace(pageM.ExtractHtml))
                    {
                        locationInfo.WikiPages.Add(
                                new LocationInfoWikiPage()
                                {
                                    DisplayTitle = pageM.DisplayTitle,
                                    ThumbnailImage = pageM.Thumbnail?.ImageURI,
                                    WikipageURL = pageM.ContentURI?["desktop"]["page"],
                                    InfoTextHtml = pageM.ExtractHtml
                                }
                            );
                    }
                }
            }

            return new JsonResult(
                    locationInfo,
                    new JsonSerializerOptions()
                    {
                        DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.Never,
                        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                        WriteIndented = false
                    }
                );
        }


        private void GpsLocate()
        {
            string? ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
            if (string.IsNullOrWhiteSpace(ipAddress) || (ipAddress == "::1") || (webHostEnvironment.IsDevelopment()))
            {
                ipAddress = "103.211.52.187";   // a delhi IP address
            }

            // honor rate limiting
            if ((_ipLocation_rateLimit_remaining <= 0) && (_ipLocation_rateLimit_secondsToReset > 0))
            {
                Thread.Sleep((_ipLocation_rateLimit_secondsToReset + 5) * 1000);
            }

            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, $"http://ip-api.com/json/{ipAddress}?fields=country,countryCode,lat,lon,currency");
            request.Headers.Add("Accept", "application/json");

            HttpClient client = new ();
            HttpResponseMessage responseMessage = client.SendAsync(request).Result;

            _ipLocation_rateLimit_secondsToReset = Convert.ToInt32(responseMessage.Headers.GetValues("X-Ttl").ToArray()[0]);
            _ipLocation_rateLimit_remaining = Convert.ToInt32(responseMessage.Headers.GetValues("X-Rl").ToArray()[0]);
            if (responseMessage.IsSuccessStatusCode)
            {
                Dictionary<string, object> response = JsonSerializer.Deserialize<Dictionary<string, object>>(responseMessage.Content.ReadAsStringAsync().Result)!;

                ViewData["lat"] = ((JsonElement)response["lat"]).GetDouble();
                ViewData["lon"] = ((JsonElement)response["lon"]).GetDouble();
                ViewData["country"] = ((JsonElement)response["country"]).GetString();
                ViewData["currency"] = ((JsonElement)response["currency"]).GetString();
            }
        }

        private void GenerateXsrf()
        {
            ViewData["_XsrfToken"] = Guid.NewGuid().ToString("n");
        }

        private bool VerifyXsrf(string token)
        {
            string? actualToken = HttpContext.Session.GetString("_XsrfToken");
            return ((!string.IsNullOrWhiteSpace(token)) && (!string.IsNullOrWhiteSpace(actualToken)) && (token == actualToken));
        }


        public MapUXController(IWebHostEnvironment env, IConfiguration config)
        {
            webHostEnvironment = env;
            configuration = config;
        }

        private IWebHostEnvironment webHostEnvironment;
        private IConfiguration configuration;
        private static int _ipLocation_rateLimit_remaining = 45;
        private static int _ipLocation_rateLimit_secondsToReset = 0;

    }
}
