using Newtonsoft.Json;

namespace ArbitrageService.Core.Models.Binance;

public class BinanceKlineResponse
{
    [JsonProperty("openTime")]
    public long OpenTime { get; set; }

    [JsonProperty("closePrice")]
    public string ClosePrice { get; set; } = string.Empty;
}
