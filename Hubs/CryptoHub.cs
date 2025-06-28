using Microsoft.AspNetCore.SignalR;

public class CryptoHub : Hub
{
    private readonly ICryptoService _cryptoService;
    private static readonly Dictionary<string, List<string>> _userSubscriptions = new();

    public CryptoHub(ICryptoService cryptoService)
    {
        _cryptoService = cryptoService;
    }

    public override async Task OnConnectedAsync()
    {
        await Clients.Caller.SendAsync("Connected", Context.ConnectionId);

      
        var marketSummary = await _cryptoService.GetMarketSummaryAsync();
        await Clients.Caller.SendAsync("MarketSummary", marketSummary);

        var topCryptos = await _cryptoService.GetTopCryptosAsync(20);
        await Clients.Caller.SendAsync("CryptoList", topCryptos);

        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        _userSubscriptions.Remove(Context.ConnectionId);
        await base.OnDisconnectedAsync(exception);
    }

    public async Task SubscribeToSymbols(List<string> symbols)
    {
        _userSubscriptions[Context.ConnectionId] = symbols;

        
        foreach (var symbol in symbols)
        {
            var price = await _cryptoService.GetCryptoPriceAsync(symbol);
            if (price != null)
            {
                await Clients.Caller.SendAsync("PriceUpdate", price);
            }
        }

        await Clients.Caller.SendAsync("SubscriptionConfirmed", symbols);
    }

    public async Task SearchCrypto(string query)
    {
        var results = await _cryptoService.SearchCryptosAsync(query);
        await Clients.Caller.SendAsync("SearchResults", results);
    }

    
    public async Task BroadcastPriceUpdate(CryptoPrice cryptoPrice)
    {
        await Clients.All.SendAsync("PriceUpdate", cryptoPrice);
    }

    public async Task BroadcastMarketAlert(MarketAlert alert)
    {
        await Clients.All.SendAsync("MarketAlert", alert);
    }
}