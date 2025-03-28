using ArbitrageService.Core.Models;

namespace ArbitrageService.Core.Interfaces;

public interface IBinanceService
{
    Task<decimal> GetFuturesPriceAsync(string symbol);
    Task<decimal> GetLastKnownPriceAsync(string symbol);
    Task<FuturesPrice> GetFuturesPriceByTimeRangeAsync(string symbol, DateTime startTime, DateTime endTime);
}
