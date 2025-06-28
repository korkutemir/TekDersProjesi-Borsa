public class MarketAlert
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Symbol { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public decimal ChangePercent { get; set; }
    public string AlertType { get; set; } = "INFO"; 
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}