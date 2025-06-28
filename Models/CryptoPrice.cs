public class CryptoPrice
{
    public string Symbol { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public decimal Change24h { get; set; }
    public decimal ChangePercent24h { get; set; }
    public decimal Volume24h { get; set; }
    public decimal MarketCap { get; set; }
    public decimal High24h { get; set; }
    public decimal Low24h { get; set; }
    public DateTime LastUpdate { get; set; } = DateTime.UtcNow;
}