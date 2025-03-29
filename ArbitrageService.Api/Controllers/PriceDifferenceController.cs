using ArbitrageService.Core.Interfaces;
using ArbitrageService.Core.Models;
using Microsoft.AspNetCore.Mvc;

namespace ArbitrageService.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PriceDifferenceController : ControllerBase
{
    private readonly IBinanceService _binanceService;
    private readonly IPriceDifferenceRepository _repository;
    private readonly ILogger<PriceDifferenceController> _logger;

    public PriceDifferenceController(
        IPriceDifferenceRepository repository,
        ILogger<PriceDifferenceController> logger,
        IBinanceService binanceService)
    {
        _repository = repository;
        _logger = logger;
        _binanceService = binanceService;
    }

    [HttpGet("latest")]
    public async Task<ActionResult<PriceDifference>> GetLatest(
        [FromQuery] string firstSymbol,
        [FromQuery] string secondSymbol)
    {
        try
        {
            var result = await _repository.GetLatestAsync(firstSymbol, secondSymbol);
            if (result == null)
            {
                return NotFound($"No price difference found for {firstSymbol} and {secondSymbol}");
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting latest price difference");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpGet("range")]
    public async Task<ActionResult<IEnumerable<PriceDifference>>> GetRange(
        [FromQuery] string firstSymbol,
        [FromQuery] string secondSymbol,
        [FromQuery] DateTime startTime,
        [FromQuery] DateTime endTime)
    {
        try
        {
            var results = await _repository.GetRangeAsync(firstSymbol, secondSymbol, startTime, endTime);
            return Ok(results);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting price difference range");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpGet("futures-price-range")]
    public async Task<ActionResult<FuturesPrice>> GetFuturesPriceByTimeRange(
        [FromQuery] string symbol,
        [FromQuery] DateTime startTime,
        [FromQuery] DateTime endTime)
    {
        try
        {
            var futuresPrice = await _binanceService.GetFuturesPriceByTimeRangeAsync(symbol, startTime, endTime);
            return Ok(futuresPrice);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при получении цены по диапазону времени для символа {Symbol}", symbol);
            return StatusCode(500, "Internal server error");
        }
    }
} 