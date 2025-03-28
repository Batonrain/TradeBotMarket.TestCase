using ArbitrageService.Core.Interfaces;
using ArbitrageService.Core.Models;
using System.Net.Http.Json;
using System.Globalization;
using ArbitrageService.Core.Models.Binance;

namespace ArbitrageService.Infrastructure.Services;

public class BinanceService : IBinanceService
{
    private readonly HttpClient _httpClient;
    private readonly Dictionary<string, decimal> _lastKnownPrices;
    private const string SpotBaseUrl = "https://api.binance.com/api/v3";
    private const string FuturesBaseUrl = "https://fapi.binance.com/fapi/v1";

    public BinanceService(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _lastKnownPrices = new Dictionary<string, decimal>();
    }

    public async Task<decimal> GetFuturesPriceAsync(string symbol)
    {
        try
        {
            var baseUrl = symbol.Contains("_") ? FuturesBaseUrl : SpotBaseUrl;
            var requestSymbol = symbol.Split('_')[0];

            var response = await _httpClient.GetFromJsonAsync<BinancePriceResponse>($"{baseUrl}/ticker/price?symbol={requestSymbol}");

            if (response == null)
            {
                return await GetLastKnownPriceAsync(symbol);
            }

            var price = decimal.Parse(response.Price, CultureInfo.InvariantCulture);
            _lastKnownPrices[symbol] = price;
            return price;
        }
        catch
        {
            return await GetLastKnownPriceAsync(symbol);
        }
    }

    public Task<decimal> GetLastKnownPriceAsync(string symbol)
    {
        return Task.FromResult(_lastKnownPrices.TryGetValue(symbol, out var price) ? price : 0m);
    }

    public async Task<FuturesPrice> GetFuturesPriceByTimeRangeAsync(string symbol, DateTime startTime, DateTime endTime)
    {
        try
        {
            var baseUrl = symbol.Contains("_") ? FuturesBaseUrl : SpotBaseUrl;
            var requestSymbol = symbol.Split('_')[0];
            
            var startTimeMillis = ((DateTimeOffset)startTime).ToUnixTimeMilliseconds();
            var endTimeMillis = ((DateTimeOffset)endTime).ToUnixTimeMilliseconds();
            
            var response = await _httpClient.GetFromJsonAsync<BinanceKlineResponse[]>(
                $"{baseUrl}/klines?symbol={requestSymbol}&interval=1h&startTime={startTimeMillis}&endTime={endTimeMillis}");

            if (response == null || !response.Any())
            {
                var lastKnownPrice = await GetLastKnownPriceAsync(symbol);
                return new FuturesPrice
                {
                    Id = Guid.NewGuid(),
                    Symbol = symbol,
                    Price = lastKnownPrice,
                    Timestamp = DateTime.UtcNow,
                    CreatedAt = DateTime.UtcNow
                };
            }

            var kline = response.First();
            return new FuturesPrice
            {
                Id = Guid.NewGuid(),
                Symbol = symbol,
                Price = decimal.Parse(kline.ClosePrice, CultureInfo.InvariantCulture),
                Timestamp = DateTimeOffset.FromUnixTimeMilliseconds(kline.OpenTime).UtcDateTime,
                CreatedAt = DateTime.UtcNow
            };
        }
        catch
        {
            var lastKnownPrice = await GetLastKnownPriceAsync(symbol);
            return new FuturesPrice
            {
                Id = Guid.NewGuid(),
                Symbol = symbol,
                Price = lastKnownPrice,
                Timestamp = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow
            };
        }
    }
} 