
using System.Text.Json.Serialization;

namespace SujaySarma.Web.TravelWeb.Data.Countries
{
    /// <summary>
    /// Country information from "countries.json"
    /// </summary>
    public class Country
    {

        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("alpha2Code")]
        public string? ISOCountryCode { get; set; }

        [JsonPropertyName("capital")]
        public string? Capital { get; set; }

        [JsonPropertyName("latlng")]
        public List<double> Coordinates { get; set; } = new() { 0.0, 0.0 };

        [JsonPropertyName("timezones")]
        public List<string>? Timezones { get; set; }

        [JsonPropertyName("currencies")]
        public List<CurrencyInfo> Currency { get; set; } = new();

    }


    public class CurrencyInfo
    {

        [JsonPropertyName("code")]
        public string? Code { get; set; }

        [JsonPropertyName("Name")]
        public string? Name { get; set; }

        [JsonPropertyName("symbol")]
        public string? UTF8Symbol { get; set; }

    }
}
