using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace CryptoBorsaApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AlertController : ControllerBase
    {
        private readonly IHubContext<CryptoHub> _hubContext;
        private static readonly List<MarketAlert> _alerts = new(); 

        public AlertController(IHubContext<CryptoHub> hubContext)
        {
            _hubContext = hubContext;
        }

        
        [HttpGet]
        public ActionResult GetAlerts([FromQuery] int count = 50)
        {
            var alerts = _alerts
                .OrderByDescending(a => a.Timestamp)
                .Take(count)
                .ToList();

            return Ok(alerts);
        }

     
        [HttpPost]
        public async Task<ActionResult> CreateAlert([FromBody] CreateAlertRequest request)
        {
            if (string.IsNullOrEmpty(request.Message))
                return BadRequest("Message is required");

            var alert = new MarketAlert
            {
                Symbol = request.Symbol ?? "GENERAL",
                Message = request.Message,
                Price = request.Price,
                ChangePercent = request.ChangePercent,
                AlertType = request.AlertType ?? "INFO"
            };

            _alerts.Add(alert);

       
            await _hubContext.Clients.All.SendAsync("MarketAlert", alert);

            return Ok(alert);
        }

       
        [HttpGet("{id}")]
        public ActionResult GetAlert(string id)
        {
            var alert = _alerts.FirstOrDefault(a => a.Id == id);
            if (alert == null)
                return NotFound();

            return Ok(alert);
        }


        [HttpDelete("{id}")]
        public ActionResult DeleteAlert(string id)
        {
            var alert = _alerts.FirstOrDefault(a => a.Id == id);
            if (alert == null)
                return NotFound();

            _alerts.Remove(alert);
            return NoContent();
        }
    }


    public class CreateAlertRequest
    {
        public string? Symbol { get; set; }
        public string Message { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public decimal ChangePercent { get; set; }
        public string? AlertType { get; set; }
    }
}