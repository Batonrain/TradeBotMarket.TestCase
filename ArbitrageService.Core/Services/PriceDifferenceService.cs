using ArbitrageService.Core.Interfaces;
using ArbitrageService.Core.Models;

namespace ArbitrageService.Core.Services;

public class PriceDifferenceService
{
    private readonly IBinanceService _binanceService;
    private readonly IPriceDifferenceRepository _repository;

    public PriceDifferenceService(
        IBinanceService binanceService,
        IPriceDifferenceRepository repository)
    {
        _binanceService = binanceService;
        _repository = repository;
    }

    public async Task<PriceDifference> CalculatePriceDifferenceAsync(string firstSymbol, string secondSymbol)
    {
        var firstPrice = await _binanceService.GetFuturesPriceAsync(firstSymbol);
        var secondPrice = await _binanceService.GetFuturesPriceAsync(secondSymbol);

        var priceDifference = new PriceDifference
        {
            Id = Guid.NewGuid(),
            FirstSymbol = firstSymbol,
            SecondSymbol = secondSymbol,
            FirstPrice = firstPrice,
            SecondPrice = secondPrice,
            Difference = secondPrice - firstPrice,
            Timestamp = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow
        };

        return await _repository.AddAsync(priceDifference);
    }
}
