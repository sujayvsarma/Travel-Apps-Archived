using System;
using System.Text;

namespace TravelIdeasPortalWeb.Objects.Skyscanner
{
    /// <summary>
    /// Data structure for Pricing Request API:
    /// https://skyscanner-skyscanner-flight-search-v1.p.rapidapi.com/apiservices/pricing/v1.0
    /// </summary>
    /// <remarks>
    ///     This structure is not demanded by the API, it is our "convenience aid"
    /// </remarks>
    public class SkyscannerPricingRequest
    {
        /// <summary>
        /// IATA code for the origin
        /// </summary>
        public SkyscannerPricingRequestHalt Origin { get; set; } = null;

        /// <summary>
        /// IATA code for the destination
        /// </summary>
        public SkyscannerPricingRequestHalt Destination { get; set; } = null;

        /// <summary>
        /// Date for departure (time is ignored)
        /// </summary>
        public DateTime OutboundDeparture { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Date for departure on the return trip (time is ignored)
        /// </summary>
        public DateTime? ReturnDeparture { get; set; } = null;

        /// <summary>
        /// Number of adults
        /// </summary>
        public int Adults { get; set; } = 1;

        /// <summary>
        /// Number of children
        /// </summary>
        public int Children { get; set; } = 0;

        /// <summary>
        /// Number of infants
        /// </summary>
        public int Infants { get; set; } = 0;

        /// <summary>
        /// Travel class
        /// </summary>
        public SkyCabinClass CabinClass { get; set; } = SkyCabinClass.Economy;

        /// <summary>
        /// User's country
        /// </summary>
        public string MarketCountryISO { get; set; } = "IN";

        /// <summary>
        /// User's currency
        /// </summary>
        public string UserCurrencyISO { get; set; } = "INR";

        /// <summary>
        /// User's locale
        /// </summary>
        public string UserLocale { get; set; } = "en-US";

        /// <summary>
        /// Returns URL-encoded string for the request. Also validates the data
        /// </summary>
        /// <returns>URL-encoded string for the GET request</returns>
        public override string ToString()
        {
            if (OutboundDeparture < DateTime.UtcNow)
            {
                throw new ArgumentException(nameof(OutboundDeparture));
            }

            if (ReturnDeparture.HasValue && (ReturnDeparture.Value < OutboundDeparture))
            {
                throw new ArgumentException(nameof(ReturnDeparture));
            }

            if (!Enum.IsDefined(typeof(SkyCabinClass), CabinClass))
            {
                throw new ArgumentException(nameof(CabinClass));
            }

            if (Adults == 0)
            {
                throw new ArgumentException(nameof(Adults));
            }

            StringBuilder result = new StringBuilder();
            result = result.Append("groupPricing=true")
                .Append("&country=").Append(MarketCountryISO)
                    .Append("&currency=").Append(UserCurrencyISO)
                        .Append("&locale=").Append(UserLocale)
                            .Append("&originPlace=").Append(Origin.SkyId)
                                .Append("&destinationPlace=").Append(Destination.SkyId)
                                    .Append("&outboundDate=").Append(OutboundDeparture.ToString("yyyy-MM-dd"))
                                        .Append("&cabinClass=").Append(Enum.GetName(typeof(SkyCabinClass), CabinClass).ToLower())
                                            .Append("&adults=").Append(Adults.ToString())
                                                .Append("&children=").Append(Children.ToString())
                                                    .Append("&infants=").Append(Infants.ToString());

            if (ReturnDeparture.HasValue)
            {
                result = result.Append("&inboundDate=").Append(ReturnDeparture.Value.ToString("yyyy-MM-dd"));
            }

            return result.ToString();
        }
    }
}
