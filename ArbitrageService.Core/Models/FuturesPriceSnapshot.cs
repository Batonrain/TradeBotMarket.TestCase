namespace ArbitrageService.Core.Models;

public class FuturesPriceSnapshot
{
    public string Symbol { get; set; } = default!;
    public decimal Price { get; set; }
    public DateTime Timestamp { get; set; }
}
