namespace ArbitrageService.Infrastructure.HttpFactory;

public interface IBinanceHttpClientFactory
{
    HttpClient CreateSpotClient();
    HttpClient CreateFuturesClient();
}
