using ArbitrageService.Core.Interfaces;
using ArbitrageService.Core.Models;
using ArbitrageService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ArbitrageService.Infrastructure.Repositories;

public class FuturesPriceRepository : IFuturesPriceRepository
{
    private readonly ApplicationDbContext _context;

    public FuturesPriceRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<FuturesPrice> GetLatestPriceAsync(string symbol)
    {
        return await _context.FuturesPrices
            .Where(p => p.Symbol == symbol)
            .OrderByDescending(p => p.Timestamp)
            .FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<FuturesPrice>> GetPricesByTimeRangeAsync(string symbol, DateTime startTime, DateTime endTime)
    {
        return await _context.FuturesPrices
            .Where(p => p.Symbol == symbol && p.Timestamp >= startTime && p.Timestamp <= endTime)
            .OrderBy(p => p.Timestamp)
            .ToListAsync();
    }

    public async Task SavePriceAsync(FuturesPrice price)
    {
        await _context.FuturesPrices.AddAsync(price);
        await _context.SaveChangesAsync();
    }

    public async Task SavePricesAsync(IEnumerable<FuturesPrice> prices)
    {
        await _context.FuturesPrices.AddRangeAsync(prices);
        await _context.SaveChangesAsync();
    }
}
