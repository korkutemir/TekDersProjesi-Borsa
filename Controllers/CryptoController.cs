using Microsoft.AspNetCore.Mvc;

namespace CryptoBorsaApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CryptoController : ControllerBase
    {
        private readonly ICryptoService _cryptoService;

        public CryptoController(ICryptoService cryptoService)
        {
            _cryptoService = cryptoService;
        }

        
        [HttpGet("top")]
        public async Task<ActionResult> GetTopCryptos([FromQuery] int count = 20)
        {
            var cryptos = await _cryptoService.GetTopCryptosAsync(count);
            return Ok(cryptos);
        }

  
        [HttpGet("{symbol}")]
        public async Task<ActionResult> GetCryptoPrice(string symbol)
        {
            var crypto = await _cryptoService.GetCryptoPriceAsync(symbol);
            if (crypto == null)
                return NotFound($"Crypto '{symbol}' not found");

            return Ok(crypto);
        }

        
        [HttpGet("search")]
        public async Task<ActionResult> SearchCryptos([FromQuery] string query)
        {
            if (string.IsNullOrEmpty(query))
                return BadRequest("Query is required");

            var results = await _cryptoService.SearchCryptosAsync(query);
            return Ok(results);
        }

 
        [HttpGet("prices")]
        public async Task<ActionResult> GetMultiplePrices([FromQuery] string symbols)
        {
            if (string.IsNullOrEmpty(symbols))
                return BadRequest("Symbols parameter is required");

            var symbolList = symbols.Split(',').Select(s => s.Trim()).ToList();
            var results = new List<CryptoPrice>();

            foreach (var symbol in symbolList)
            {
                var crypto = await _cryptoService.GetCryptoPriceAsync(symbol);
                if (crypto != null)
                    results.Add(crypto);
            }

            return Ok(results);
        }
    }
}