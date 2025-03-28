using ArbitrageService.Core.Interfaces;
using ArbitrageService.Core.Models;
using Quartz;

namespace ArbitrageService.Worker.Jobs;

public class PriceDifferenceJob : IJob
{
    private readonly IBinanceService _binanceService;
    private readonly IPriceDifferenceRepository _repository;
    private readonly ILogger<PriceDifferenceJob> _logger;

    public PriceDifferenceJob(
        IBinanceService binanceService,
        IPriceDifferenceRepository repository,
        ILogger<PriceDifferenceJob> logger)
    {
        _binanceService = binanceService;
        _repository = repository;
        _logger = logger;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        try
        {
            const string firstSymbol = "BTCUSDT";
            const string secondSymbol = "BTCUSDT_240628";

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

            await _repository.AddAsync(priceDifference);
            _logger.LogInformation("Price difference saved: {FirstSymbol} - {SecondSymbol} = {Difference}",
                firstSymbol, secondSymbol, priceDifference.Difference);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error executing PriceDifferenceJob");
            throw;
        }
    }
} 