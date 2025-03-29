using ArbitrageService.Core.Interfaces;
using ArbitrageService.Core.Models;
using System.Net.Http.Json;
using System.Globalization;
using ArbitrageService.Core.Models.Binance;
using ArbitrageService.Infrastructure.HttpFactory;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace ArbitrageService.Infrastructure.Services;

public class BinanceService : IBinanceService
{
    private readonly HttpClient _spotClient;
    private readonly HttpClient _futuresClient;
    private readonly Dictionary<string, decimal> _lastKnownPrices = new Dictionary<string, decimal>();
    private readonly ILogger<BinanceService> _logger;

    public BinanceService(IBinanceHttpClientFactory clientFactory, ILogger<BinanceService> logger)
    {
        _spotClient = clientFactory.CreateSpotClient();
        _futuresClient = clientFactory.CreateFuturesClient();
        _logger = logger;
    }

    public async Task<decimal> GetFuturesPriceAsync(string symbol)
    {
        try
        {
            _logger.LogInformation("��������� ���� ��� �������: {Symbol}", symbol);
            var client = symbol.Contains("_") ? _spotClient : _futuresClient;
            var requestSymbol = symbol.Split('_')[0];

            var response = await client.GetFromJsonAsync<BinancePriceResponse>($"ticker/price?symbol={requestSymbol}");

            if (response == null)
            {
                _logger.LogWarning("��� ������ �� ������� ��� ������� {Symbol}, ���������� ��������� ��������� ����", symbol);
                return await GetLastKnownPriceAsync(symbol);
            }

            var price = decimal.Parse(response.Price, CultureInfo.InvariantCulture);
            _lastKnownPrices[symbol] = price;
            _logger.LogInformation("�������� ���� {Price} ��� ������� {Symbol}", price, symbol);
            return price;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "������ ��� ��������� ���� ��� ������� {Symbol}. ������� ��������� ��������� ����.", symbol);
            return await GetLastKnownPriceAsync(symbol);
        }
    }

    public Task<decimal> GetLastKnownPriceAsync(string symbol)
    {
        if (_lastKnownPrices.TryGetValue(symbol, out var price))
        {
            _logger.LogDebug("���������� ��������� ��������� ���� {Price} ��� ������� {Symbol}", price, symbol);
            return Task.FromResult(price);
        }
        _logger.LogDebug("��������� ��������� ���� ��� ������� {Symbol} �� �������, ���������� 0", symbol);
        return Task.FromResult(0m);
    }

    public async Task<FuturesPriceSnapshot> GetFuturesPriceByTimeRangeAsync(string symbol, DateTime startTime, DateTime endTime)
    {
        try
        {
            _logger.LogInformation("��������� ���� �� ��������� ������� ��� ������� {Symbol} ����� {StartTime} � {EndTime}", symbol, startTime, endTime);

            var client = symbol.Contains("_") ? _spotClient : _futuresClient;
            var requestSymbol = symbol.Split('_')[0];

            var startTimeMillis = ((DateTimeOffset)startTime).ToUnixTimeMilliseconds();
            var endTimeMillis = ((DateTimeOffset)endTime).ToUnixTimeMilliseconds();

            var url = $"klines?symbol={requestSymbol}&interval=1h&startTime={startTimeMillis}&endTime={endTimeMillis}";
            var response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            var rawData = JsonSerializer.Deserialize<List<List<JsonElement>>>(json);

            if (rawData == null || rawData.Count == 0)
            {
                _logger.LogWarning("��� ������ �� ������ ��� ������� {Symbol}, ���������� ��������� ��������� ����", symbol);
                var lastKnownPrice = await GetLastKnownPriceAsync(symbol);
                return new FuturesPriceSnapshot
                {
                    Symbol = symbol,
                    Price = lastKnownPrice,
                    Timestamp = DateTime.UtcNow,
                };
            }

            var first = rawData.First();

            var openTime = DateTimeOffset.FromUnixTimeMilliseconds(first[0].GetInt64()).UtcDateTime;
            var closePrice = decimal.Parse(first[4].GetString(), CultureInfo.InvariantCulture);

            _logger.LogInformation("�������� ������ ����� ��� ������� {Symbol}, ���� {Price}", symbol, closePrice);

            return new FuturesPriceSnapshot
            {
                Symbol = symbol,
                Price = closePrice,
                Timestamp = openTime,
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "������ ��� ��������� ���� �� ��������� ������� ��� ������� {Symbol}. ���������� ��������� ��������� ����.", symbol);
            var lastKnownPrice = await GetLastKnownPriceAsync(symbol);
            return new FuturesPriceSnapshot
            {
                Symbol = symbol,
                Price = lastKnownPrice,
                Timestamp = DateTime.UtcNow,
            };
        }
    }
}
