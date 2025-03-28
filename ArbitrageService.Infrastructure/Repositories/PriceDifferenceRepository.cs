using ArbitrageService.Core.Interfaces;
using ArbitrageService.Core.Models;
using ArbitrageService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ArbitrageService.Infrastructure.Repositories;

public class PriceDifferenceRepository : IPriceDifferenceRepository
{
    private readonly ApplicationDbContext _context;

    public PriceDifferenceRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PriceDifference?> GetLatestAsync(string firstSymbol, string secondSymbol)
    {
        return await _context.PriceDifferences
            .Where(pd => pd.FirstSymbol == firstSymbol && pd.SecondSymbol == secondSymbol)
            .OrderByDescending(pd => pd.Timestamp)
            .FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<PriceDifference>> GetRangeAsync(string firstSymbol, string secondSymbol, DateTime startTime, DateTime endTime)
    {
        return await _context.PriceDifferences
            .Where(pd => pd.FirstSymbol == firstSymbol && 
                        pd.SecondSymbol == secondSymbol &&
                        pd.Timestamp >= startTime && 
                        pd.Timestamp <= endTime)
            .OrderBy(pd => pd.Timestamp)
            .ToListAsync();
    }

    public async Task<PriceDifference> AddAsync(PriceDifference priceDifference)
    {
        _context.PriceDifferences.Add(priceDifference);
        await _context.SaveChangesAsync();
        return priceDifference;
    }

    public async Task SaveDifferenceAsync(PriceDifference difference)
    {
        await _context.PriceDifferences.AddAsync(difference);
        await _context.SaveChangesAsync();
    }

    public async Task SaveDifferencesAsync(IEnumerable<PriceDifference> differences)
    {
        await _context.PriceDifferences.AddRangeAsync(differences);
        await _context.SaveChangesAsync();
    }
}
