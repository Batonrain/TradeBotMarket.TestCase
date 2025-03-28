using ArbitrageService.Core.Models;

namespace ArbitrageService.Core.Interfaces;

public interface IPriceDifferenceRepository
{
    Task<PriceDifference?> GetLatestAsync(string firstSymbol, string secondSymbol);
    Task<IEnumerable<PriceDifference>> GetRangeAsync(string firstSymbol, string secondSymbol, DateTime startTime, DateTime endTime);
    Task<PriceDifference> AddAsync(PriceDifference priceDifference);
}
