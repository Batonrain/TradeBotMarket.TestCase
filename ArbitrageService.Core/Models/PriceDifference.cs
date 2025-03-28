namespace ArbitrageService.Core.Models;

public class PriceDifference
{
    public Guid Id { get; set; }
    public string FirstSymbol { get; set; } = string.Empty;
    public string SecondSymbol { get; set; } = string.Empty;
    public decimal FirstPrice { get; set; }
    public decimal SecondPrice { get; set; }
    public decimal Difference { get; set; }
    public DateTime Timestamp { get; set; }
    public DateTime CreatedAt { get; set; }
}
