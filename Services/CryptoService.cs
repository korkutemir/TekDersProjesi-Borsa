public class CryptoService : ICryptoService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<CryptoService> _logger;
    private static readonly Dictionary<string, CryptoPrice> _priceCache = new();
    private static DateTime _lastCacheUpdate = DateTime.MinValue;

    public CryptoService(HttpClient httpClient, ILogger<CryptoService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<List<CryptoPrice>> GetTopCryptosAsync(int count = 20)
    {
        try
        {
           
            var cryptos = GetSimulatedCryptos().Take(count).ToList();

         
            foreach (var crypto in cryptos)
            {
                _priceCache[crypto.Symbol] = crypto;
            }
            _lastCacheUpdate = DateTime.UtcNow;

            return await Task.FromResult(cryptos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching top cryptos");
            return new List<CryptoPrice>();
        }
    }

    public async Task<CryptoPrice?> GetCryptoPriceAsync(string symbol)
    {
        try
        {
            
            if (_priceCache.ContainsKey(symbol.ToUpper()) &&
                DateTime.UtcNow - _lastCacheUpdate < TimeSpan.FromSeconds(30))
            {
                return await Task.FromResult(_priceCache[symbol.ToUpper()]);
            }

           
            var price = GenerateSimulatedPrice(symbol.ToUpper());
            _priceCache[symbol.ToUpper()] = price;

            return await Task.FromResult(price);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching price for {Symbol}", symbol);
            return null;
        }
    }

    public async Task<MarketSummary> GetMarketSummaryAsync()
    {
        try
        {
            var topCryptos = await GetTopCryptosAsync(10);
            var gainers = topCryptos.Where(c => c.ChangePercent24h > 0)
                                   .OrderByDescending(c => c.ChangePercent24h)
                                   .Take(5).ToList();
            var losers = topCryptos.Where(c => c.ChangePercent24h < 0)
                                  .OrderBy(c => c.ChangePercent24h)
                                  .Take(5).ToList();

            return new MarketSummary
            {
                TotalMarketCap = 2_150_000_000_000m,
                TotalVolume24h = 95_000_000_000m,
                MarketCapChange24h = -2.3m,
                ActiveCryptocurrencies = 13_245,
                DominantCrypto = "BTC",
                DominancePercentage = 52.1m,
                TopGainers = gainers,
                TopLosers = losers
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching market summary");
            return new MarketSummary();
        }
    }

    public async Task<List<CryptoPrice>> SearchCryptosAsync(string query)
    {
        try
        {
            var allCryptos = GetSimulatedCryptos();
            var results = allCryptos
                .Where(c => c.Name.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                           c.Symbol.Contains(query, StringComparison.OrdinalIgnoreCase))
                .Take(10)
                .ToList();

            return await Task.FromResult(results);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching cryptos for {Query}", query);
            return new List<CryptoPrice>();
        }
    }

    private List<CryptoPrice> GetSimulatedCryptos()
    {
        var cryptoData = new Dictionary<string, (string name, decimal basePrice)>
        {
            ["BTC"] = ("Bitcoin", 65000m),
            ["ETH"] = ("Ethereum", 3500m),
            ["BNB"] = ("BNB", 400m),
            ["XRP"] = ("XRP", 0.6m),
            ["ADA"] = ("Cardano", 1.2m),
            ["DOGE"] = ("Dogecoin", 0.15m),
            ["MATIC"] = ("Polygon", 1.1m),
            ["SOL"] = ("Solana", 150m),
            ["DOT"] = ("Polkadot", 25m),
            ["AVAX"] = ("Avalanche", 35m),
            ["SHIB"] = ("Shiba Inu", 0.000025m),
            ["LTC"] = ("Litecoin", 180m),
            ["TRX"] = ("TRON", 0.08m),
            ["UNI"] = ("Uniswap", 12m),
            ["ATOM"] = ("Cosmos", 15m),
            ["LINK"] = ("Chainlink", 18m),
            ["XLM"] = ("Stellar", 0.25m),
            ["ICP"] = ("Internet Computer", 8m),
            ["FIL"] = ("Filecoin", 6m),
            ["THETA"] = ("Theta Network", 2.5m)
        };

        var cryptos = new List<CryptoPrice>();
        var random = new Random();

        foreach (var crypto in cryptoData)
        {
            var changePercent = (decimal)(random.NextDouble() * 20 - 10); // -10% to +10%
            var price = crypto.Value.basePrice * (1 + changePercent / 100);
            var change24h = crypto.Value.basePrice * changePercent / 100;

            cryptos.Add(new CryptoPrice
            {
                Symbol = crypto.Key,
                Name = crypto.Value.name,
                Price = Math.Round(price, crypto.Key == "BTC" ? 2 : 6),
                Change24h = Math.Round(change24h, 6),
                ChangePercent24h = Math.Round(changePercent, 2),
                Volume24h = (decimal)(random.NextDouble() * 1_000_000_000),
                MarketCap = price * (decimal)(random.NextDouble() * 1_000_000_000),
                High24h = Math.Round(price * 1.1m, 6),
                Low24h = Math.Round(price * 0.9m, 6),
                LastUpdate = DateTime.UtcNow
            });
        }

        return cryptos.OrderByDescending(c => c.MarketCap).ToList();
    }

    private CryptoPrice GenerateSimulatedPrice(string symbol)
    {
        var cryptoData = GetSimulatedCryptos();
        return cryptoData.FirstOrDefault(c => c.Symbol == symbol) ??
               new CryptoPrice { Symbol = symbol, Name = symbol, Price = 1m };
    }
}