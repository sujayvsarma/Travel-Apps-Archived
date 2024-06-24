using SujaySarma.Sdk.AspNetCore.Mvc;
using SujaySarma.Sdk.RestApi;

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;

using TravelIdeasPortalWeb.Objects.Skyscanner;

namespace TravelIdeasPortalWeb.ServiceClients
{
    public static class SkyscannerClient
    {
        /// <summary>
        /// (Thread safe) Get the Skyscanner Id for the place
        /// </summary>
        /// <param name="localPlaceName">Local name of the place</param>
        /// <param name="isoCountry">The country the place is in</param>
        /// <returns>The SkyId of the place</returns>
        public static string GetSkyscannerPlaceId(string localPlaceName, string isoCountry)
            => GetSkyscannerPlace(localPlaceName, isoCountry)?.SkyId;

        /// <summary>
        /// (Thread safe) Get the SkyscannerPlace for the place
        /// </summary>
        /// <param name="nameOrIata">Local name or IATA code of the place</param>
        /// <param name="isoCountry">The country the place is in</param>
        /// <returns>The SkyId of the place</returns>
        public static SkyscannerPlace GetSkyscannerPlace(string nameOrIata, string isoCountry)
            => _placesCache.GetOrAdd(
                    nameOrIata,
                    (ni) =>
                    {
                        RestApiClient client = new RestApiClient(new Uri($"https://skyscanner-skyscanner-flight-search-v1.p.rapidapi.com/apiservices/autosuggest/v1.0/{isoCountry}/INR/en-US/?query={nameOrIata}"));
                        HttpResponseMessage responseMessage = AddApiKeyHeader(client).Get();
                        if (responseMessage.IsSuccessStatusCode)
                        {
                            SkyscannerPlaceList result = JsonSerializer.Deserialize<SkyscannerPlaceList>(responseMessage.Content.ReadAsStringAsync().Result);
                            if (result.Items.Count > 0)
                            {
                                return result.Items[0];
                            }
                        }

                        return null;
                    }
                );

        /// <summary>
        /// Get a list of ticketable quotes. These are to be displayed with "Book Now" buttons on the UX.
        /// NB: We only return AIRLINE provided rates and not tour operators
        /// </summary>
        /// <param name="pricingRequest">Structure containing data for the pricing</param>
        /// <returns>List of ticketing-ready quotes</returns>
        public static List<PricedItineraryOption> GetTicketableQuotes(SkyscannerPricingRequest pricingRequest)
        {
            if (pricingRequest.Origin.SkyId != null)
            {
                pricingRequest.Origin.SkyId = GetSkyscannerPlaceId(pricingRequest.Origin.Iata, pricingRequest.Origin.IsoCountryCode);
            }
            if (pricingRequest.Destination.SkyId != null)
            {
                pricingRequest.Destination.SkyId = GetSkyscannerPlaceId(pricingRequest.Destination.Iata, pricingRequest.Destination.IsoCountryCode);
            }

            RestApiClient client = new RestApiClient(new Uri("https://skyscanner-skyscanner-flight-search-v1.p.rapidapi.com/apiservices/pricing/v1.0"))
            {
                RequestBodyString = pricingRequest.ToString()
            };
            HttpResponseMessage responseMessage = AddApiKeyHeader(client).CallApiMethod(HttpMethod.Post, "application/x-www-form-urlencoded");
            if ((responseMessage.StatusCode != System.Net.HttpStatusCode.Created) || (responseMessage.Headers.Location == null))
            {
                return null;
            }

            // the first call creates a session, now get the session Id
            string sessionKey = responseMessage.Headers.Location.Segments[^1].Replace("/", "");
            RestApiClient responsePollClient = new RestApiClient(new Uri($"https://skyscanner-skyscanner-flight-search-v1.p.rapidapi.com/apiservices/pricing/uk2/v1.0/{sessionKey}?sortType=outbounddeparttime&sortOrder=asc"))
            {
                RequestTimeout = 30
            };

            responsePollClient = AddApiKeyHeader(responsePollClient);
            responseMessage = responsePollClient.Get();

            if (!responseMessage.IsSuccessStatusCode)
            {
                return null;
            }

            SkyscannerPricingResult pricingResult = JsonSerializer.Deserialize<SkyscannerPricingResult>(responseMessage.Content.ReadAsStringAsync().Result);

            if ((pricingResult == null)
                || ((pricingResult.Itineraries == null) || (pricingResult.Itineraries.Count == 0))
                || ((pricingResult.Legs == null) || (pricingResult.Legs.Count == 0))
                || ((pricingResult.Segments == null) || (pricingResult.Segments.Count == 0))
                || ((pricingResult.Places == null) || (pricingResult.Places.Count == 0))
                || ((pricingResult.Carriers == null) || (pricingResult.Carriers.Count == 0))
                || ((pricingResult.Agents == null) || (pricingResult.Agents.Count == 0))
            )
            {
                return null;
            }

            // caches
            Dictionary<int, SkyscannerPricingAgent> airlineAgents = new Dictionary<int, SkyscannerPricingAgent>();
            Dictionary<string, SkyscannerPricingLeg> legs = new Dictionary<string, SkyscannerPricingLeg>();
            Dictionary<int, SkyscannerPricingSegment> segments = new Dictionary<int, SkyscannerPricingSegment>();
            Dictionary<int, SkyscannerPricingCarrier> carriers = new Dictionary<int, SkyscannerPricingCarrier>();
            Dictionary<int, Objects.OurAirport.OurAirportInfoResult> fullAirportMetadata = new Dictionary<int, Objects.OurAirport.OurAirportInfoResult>();

            // Build the cache
            foreach (SkyscannerPricingAgent item in pricingResult.Agents)
            {
                if (item.Type == "Airline")
                {
                    airlineAgents.Add(item.Id, item);
                }
            }
            foreach (SkyscannerPricingCarrier item in pricingResult.Carriers)
            {
                carriers.Add(item.Id, item);
            }
            foreach (SkyscannerPricingPlace item in pricingResult.Places)
            {
                Objects.OurAirport.OurAirportInfoResult airportInfoResult = OurAirportsApiClient.AirportSearch(item.IATA, true);
                if (airportInfoResult != null)
                {
                    fullAirportMetadata.Add(item.Id, airportInfoResult);
                }
            }
            foreach (SkyscannerPricingLeg item in pricingResult.Legs)
            {
                if (!legs.ContainsKey(item.Id))
                {
                    legs.Add(item.Id, item);
                }
            }
            foreach (SkyscannerPricingSegment item in pricingResult.Segments)
            {
                if (
                        fullAirportMetadata.ContainsKey(item.Origin) && fullAirportMetadata.ContainsKey(item.Destination)
                        && carriers.ContainsKey(item.CarrierID)
                   )
                {
                    segments.Add(item.Id, item);
                }
            }

            // if this is empty, we will not be able to populate anything from any itinerary anyway
            if (airlineAgents.Count == 0)
            {
                return null;
            }

            // this is what we return
            List<PricedItineraryOption> itineraryOptions = new List<PricedItineraryOption>();

            // preallocate - these are all used within a nested loop structure below
            SkyscannerPricingLeg outboundLeg, inboundLeg;
            SkyscannerPricingSegment segment;
            Objects.OurAirport.OurAirportInfoResult originAirport, destinationAirport;
            SkyscannerPricingCarrier carrier;

            foreach (SkyscannerPricingItinerary itinerary in pricingResult.Itineraries)
            {
                outboundLeg = legs[itinerary.OutboundLegID];
                inboundLeg = ((itinerary.InboundLegID != null) ? legs[itinerary.InboundLegID] : null);
                if ((outboundLeg == null) || (pricingRequest.ReturnDeparture.HasValue && (inboundLeg == null)))
                {
                    continue;
                }

                foreach (SkyscannerPricingOption pricingOption in itinerary.Pricing)
                {
                    foreach (int optionAgentId in pricingOption.Agents)
                    {
                        if (!airlineAgents.TryGetValue(optionAgentId, out SkyscannerPricingAgent agent))
                        {
                            break;
                        }

                        bool skipAgent = false;

                        PricedItineraryOption resultOption = new PricedItineraryOption()
                        {
                            Id = Guid.NewGuid(),
                            IsRoundTrip = ((itinerary.OutboundLegID != null) && (itinerary.InboundLegID != null)),
                            Price = pricingOption.Price,
                            TicketingUri = pricingOption.BookingURI,
                            ForwardSegments = new List<PricedItinerarySegment>(),
                            ReturnSegments = ((itinerary.InboundLegID != null) ? new List<PricedItinerarySegment>() : null),
                            Agent = new KeyValuePair<string, string>(agent.Name, agent.LogoURI),
                            TotalInboundDuration = 0,
                            TotalOutboundDuration = 0
                        };

                        foreach (int segmentId in outboundLeg.Segments)
                        {
                            // since we only resolved segments that had all its components, 
                            // this one check is sufficient!
                            if (!segments.TryGetValue(segmentId, out segment))
                            {
                                skipAgent = true;
                                break;
                            }

                            originAirport = fullAirportMetadata[segment.Origin];
                            destinationAirport = fullAirportMetadata[segment.Destination];
                            carrier = carriers[segment.CarrierID];

                            resultOption.TotalOutboundDuration += segment.DurationInMinutes;
                            resultOption.ForwardSegments.Add(
                                    new PricedItinerarySegment()
                                    {
                                        Arrival = segment.Arrival,
                                        Departure = segment.Departure,
                                        Origin = new ItineraryHalt()
                                        {
                                            IATA = originAirport.Airport.Iata,
                                            Name = originAirport.Airport.Name,
                                            CountryName = originAirport.Country.Name,
                                            IsoCountryCode = originAirport.Country.Code,
                                        },
                                        Destination = new ItineraryHalt()
                                        {
                                            IATA = destinationAirport.Airport.Iata,
                                            Name = destinationAirport.Airport.Name,
                                            CountryName = destinationAirport.Country.Name,
                                            IsoCountryCode = destinationAirport.Country.Code,
                                        },
                                        Duration = segment.DurationInMinutes,
                                        Flight = new ItineraryFlight()
                                        {
                                            IATA = carrier.IATA,
                                            LogoURI = carrier.LogoURI,
                                            Name = carrier.Name,
                                            FlightNumber = $"{carrier.DisplayIATA} {segment.FlightNumber}"
                                        }
                                    }
                                );
                        }

                        if ((!skipAgent) && (inboundLeg != null))
                        {
                            foreach (int segmentId in inboundLeg.Segments)
                            {
                                // since we only resolved segments that had all its components, 
                                // this one check is sufficient!
                                if (!segments.TryGetValue(segmentId, out segment))
                                {
                                    skipAgent = true;
                                    break;
                                }

                                originAirport = fullAirportMetadata[segment.Origin];
                                destinationAirport = fullAirportMetadata[segment.Destination];
                                carrier = carriers[segment.CarrierID];

                                resultOption.TotalInboundDuration += segment.DurationInMinutes;
                                resultOption.ReturnSegments.Add(
                                        new PricedItinerarySegment()
                                        {
                                            Arrival = segment.Arrival,
                                            Departure = segment.Departure,
                                            Origin = new ItineraryHalt()
                                            {
                                                IATA = originAirport.Airport.Iata,
                                                Name = originAirport.Airport.Name,
                                                CountryName = originAirport.Country.Name,
                                                IsoCountryCode = originAirport.Country.Code,
                                            },
                                            Destination = new ItineraryHalt()
                                            {
                                                IATA = destinationAirport.Airport.Iata,
                                                Name = destinationAirport.Airport.Name,
                                                CountryName = destinationAirport.Country.Name,
                                                IsoCountryCode = destinationAirport.Country.Code,
                                            },
                                            Duration = segment.DurationInMinutes,
                                            Flight = new ItineraryFlight()
                                            {
                                                IATA = carrier.IATA,
                                                LogoURI = carrier.LogoURI,
                                                Name = carrier.Name,
                                                FlightNumber = $"{carrier.DisplayIATA} {segment.FlightNumber}"
                                            }
                                        }
                                    );
                            }
                        }

                        if (skipAgent)
                        {
                            break;
                        }

                        // if we are still here, we have a good option
                        itineraryOptions.Add(resultOption);
                    }
                }
            }

#pragma warning disable IDE0059                             // Unnecessary assignment of a value
            // release all the memory we've wasted
            pricingResult = null;
            fullAirportMetadata = null;
            legs = null;
            segments = null;
            carriers = null;
            outboundLeg = null;
            inboundLeg = null;
            segment = null;
            originAirport = null;
            destinationAirport = null;
            carrier = null;
#pragma warning restore IDE0059                             // Unnecessary assignment of a value

            return itineraryOptions;
        }

        /// <summary>
        /// Add the API key header to the client
        /// </summary>
        /// <param name="client">RestApiClient instance</param>
        /// <returns>RestApiClient instance</returns>
        private static RestApiClient AddApiKeyHeader(RestApiClient client)
        {
            if (!client.RequestHeaders.ContainsKey("X-RapidApi-Host"))
            {
                client.RequestHeaders.Add("X-RapidApi-Host", client.RequestUri.Host);
            }

            if (!client.RequestHeaders.ContainsKey("X-RapidApi-Key"))
            {
                client.RequestHeaders.Add("X-RapidApi-Key", API_KEY);
            }

            return client;
        }

        /// <summary>
        /// Initialize the client
        /// </summary>
        static SkyscannerClient()
        {
            API_KEY = AppSettingsJson.Configuration.GetSection("rapidApi")["apiKey"];
        }

        private static readonly string API_KEY = null;  // populated in constructor
        private static readonly ConcurrentDictionary<string, SkyscannerPlace> _placesCache = new ConcurrentDictionary<string, SkyscannerPlace>();
    }
}
