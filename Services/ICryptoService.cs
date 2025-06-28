public interface ICryptoService
{
    Task<List<CryptoPrice>> GetTopCryptosAsync(int count = 20);
    Task<CryptoPrice?> GetCryptoPriceAsync(string symbol);
    Task<MarketSummary> GetMarketSummaryAsync();
    Task<List<CryptoPrice>> SearchCryptosAsync(string query);
}