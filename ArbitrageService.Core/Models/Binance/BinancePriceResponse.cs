using Newtonsoft.Json;

namespace ArbitrageService.Core.Models.Binance;

public class BinancePriceResponse
{
    [JsonProperty("symbol")]
    public string Symbol { get; set; } = string.Empty;

    [JsonProperty("price")]
    public string Price { get; set; } = string.Empty;
}
