namespace ArbitrageService.Infrastructure.HttpFactory;

public class BinanceHttpClientFactory : IBinanceHttpClientFactory
{
    private readonly IHttpClientFactory _httpClientFactory;

    public BinanceHttpClientFactory(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public HttpClient CreateSpotClient()
    {
        var client = _httpClientFactory.CreateClient("BinanceSpotClient");
        return client;
    }

    public HttpClient CreateFuturesClient()
    {
        var client = _httpClientFactory.CreateClient("BinanceFuturesClient");
        return client;
    }
}
