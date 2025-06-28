using Microsoft.AspNetCore.Mvc;

namespace CryptoBorsaApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MarketController : ControllerBase
    {
        private readonly ICryptoService _cryptoService;

        public MarketController(ICryptoService cryptoService)
        {
            _cryptoService = cryptoService;
        }

    
        [HttpGet("summary")]
        public async Task<ActionResult> GetMarketSummary()
        {
            var summary = await _cryptoService.GetMarketSummaryAsync();
            return Ok(summary);
        }

 
        [HttpGet("gainers")]
        public async Task<ActionResult> GetTopGainers([FromQuery] int count = 10)
        {
            var topCryptos = await _cryptoService.GetTopCryptosAsync(100);
            var gainers = topCryptos
                .Where(c => c.ChangePercent24h > 0)
                .OrderByDescending(c => c.ChangePercent24h)
                .Take(count)
                .ToList();

            return Ok(gainers);
        }

        
        [HttpGet("losers")]
        public async Task<ActionResult> GetTopLosers([FromQuery] int count = 10)
        {
            var topCryptos = await _cryptoService.GetTopCryptosAsync(100);
            var losers = topCryptos
                .Where(c => c.ChangePercent24h < 0)
                .OrderBy(c => c.ChangePercent24h)
                .Take(count)
                .ToList();

            return Ok(losers);
        }

     
        [HttpGet("stats")]
        public async Task<ActionResult> GetMarketStats()
        {
            var topCryptos = await _cryptoService.GetTopCryptosAsync(100);

            var stats = new
            {
                TotalCryptos = topCryptos.Count,
                PositiveChange = topCryptos.Count(c => c.ChangePercent24h > 0),
                NegativeChange = topCryptos.Count(c => c.ChangePercent24h < 0),
                AverageChange = topCryptos.Average(c => c.ChangePercent24h),
                TotalVolume = topCryptos.Sum(c => c.Volume24h),
                TotalMarketCap = topCryptos.Sum(c => c.MarketCap)
            };

            return Ok(stats);
        }
    }
}