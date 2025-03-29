using ArbitrageService.Core.Interfaces;
using ArbitrageService.Infrastructure.HttpFactory;
using ArbitrageService.Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;

namespace ArbitrageService.Infrastructure;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddBinanceHttpService(this IServiceCollection services)
    {
        services.AddHttpClient("BinanceSpotClient", client =>
        {
            client.BaseAddress = new Uri("https://api.binance.com/api/v3/");
        });

        services.AddHttpClient("BinanceFuturesClient", client =>
        {
            client.BaseAddress = new Uri("https://fapi.binance.com/fapi/v1/");
        });

        services.AddSingleton<IBinanceHttpClientFactory, BinanceHttpClientFactory>();
        services.AddTransient<IBinanceService, BinanceService>();

        return services;
    }

    
}
