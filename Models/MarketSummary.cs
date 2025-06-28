public class MarketSummary
{
    public decimal TotalMarketCap { get; set; }
    public decimal TotalVolume24h { get; set; }
    public decimal MarketCapChange24h { get; set; }
    public int ActiveCryptocurrencies { get; set; }
    public string DominantCrypto { get; set; } = "BTC";
    public decimal DominancePercentage { get; set; }
    public List<CryptoPrice> TopGainers { get; set; } = new();
    public List<CryptoPrice> TopLosers { get; set; } = new();
}