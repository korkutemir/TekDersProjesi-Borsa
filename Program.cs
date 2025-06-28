var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();
builder.Services.AddSignalR();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddHttpClient<ICryptoService, CryptoService>(client =>
{
    client.BaseAddress = new Uri("https://api.coingecko.com/api/v3/");
    client.DefaultRequestHeaders.Add("User-Agent", "CryptoBorsaApp/1.0");
    client.Timeout = TimeSpan.FromSeconds(30);
});


builder.Services.AddScoped<ICryptoService, CryptoService>();
builder.Services.AddHostedService<CryptoBackgroundService>();


builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("https://localhost:7000", "http://localhost:5000")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});


builder.Services.AddMemoryCache();

var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseCors();

app.UseRouting();
app.UseAuthorization();

app.MapControllers();

app.MapHub<CryptoHub>("/cryptohub");


app.MapGet("/api/crypto/top", async (ICryptoService cryptoService, int count = 20) =>
{
    var cryptos = await cryptoService.GetTopCryptosAsync(count);
    return Results.Ok(cryptos);
});

app.MapGet("/api/crypto/{symbol}", async (string symbol, ICryptoService cryptoService) =>
{
    var crypto = await cryptoService.GetCryptoPriceAsync(symbol);
    return crypto != null ? Results.Ok(crypto) : Results.NotFound();
});

app.MapGet("/api/market/summary", async (ICryptoService cryptoService) =>
{
    var summary = await cryptoService.GetMarketSummaryAsync();
    return Results.Ok(summary);
});

app.MapGet("/health", () => Results.Ok(new { Status = "Healthy", Timestamp = DateTime.UtcNow }));


app.MapFallbackToFile("index.html");

app.Run();