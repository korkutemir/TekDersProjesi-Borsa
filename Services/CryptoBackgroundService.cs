using Microsoft.AspNetCore.SignalR;

public class CryptoBackgroundService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IHubContext<CryptoHub> _hubContext;
    private readonly ILogger<CryptoBackgroundService> _logger;

    public CryptoBackgroundService(
        IServiceScopeFactory scopeFactory,
        IHubContext<CryptoHub> hubContext,
        ILogger<CryptoBackgroundService> logger)
    {
        _scopeFactory = scopeFactory;
        _hubContext = hubContext;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Crypto Background Service started");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();
                var cryptoService = scope.ServiceProvider.GetRequiredService<ICryptoService>();

     
                var topCryptos = await cryptoService.GetTopCryptosAsync(20);
                await _hubContext.Clients.All.SendAsync("CryptoListUpdate", topCryptos, stoppingToken);

              
                var marketSummary = await cryptoService.GetMarketSummaryAsync();
                await _hubContext.Clients.All.SendAsync("MarketSummaryUpdate", marketSummary, stoppingToken);

           
                if (new Random().NextDouble() < 0.1) 
                {
                    var alert = GenerateRandomAlert();
                    await _hubContext.Clients.All.SendAsync("MarketAlert", alert, stoppingToken);
                }

                _logger.LogInformation("Updated crypto data for all clients");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in crypto background service");
            }

     
            await Task.Delay(2000, stoppingToken);
        }
    }

    private MarketAlert GenerateRandomAlert()
    {
        var alerts = new[]
        {
            "🚀 Bitcoin surged above $70,000!",
            "📈 Ethereum showing strong bullish momentum",
            "⚡ New all-time high trading volume detected",
            "💎 Major institutional investment in crypto market",
            "🌟 DeFi sector experiencing significant growth"
        };

        var symbols = new[] { "BTC", "ETH", "BNB", "ADA", "SOL" };
        var alertTypes = new[] { "INFO", "WARNING", "CRITICAL" };

        var random = new Random();

        return new MarketAlert
        {
            Symbol = symbols[random.Next(symbols.Length)],
            Message = alerts[random.Next(alerts.Length)],
            Price = (decimal)(random.NextDouble() * 100000),
            ChangePercent = (decimal)(random.NextDouble() * 20 - 10),
            AlertType = alertTypes[random.Next(alertTypes.Length)]
        };
    }
}