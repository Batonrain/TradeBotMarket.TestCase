using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ArbitrageService.Core.Models;

namespace ArbitrageService.Core.Interfaces;

public interface IFuturesPriceRepository
{
    Task<FuturesPrice> GetLatestPriceAsync(string symbol);
    Task<IEnumerable<FuturesPrice>> GetPricesByTimeRangeAsync(string symbol, DateTime startTime, DateTime endTime);
    Task SavePriceAsync(FuturesPrice price);
    Task SavePricesAsync(IEnumerable<FuturesPrice> prices);
}
