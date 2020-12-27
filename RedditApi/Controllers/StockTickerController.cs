using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Common.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RedditApi.Logic;

namespace RedditApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class StockTickerController : ControllerBase
    {
        private readonly ILogger<StockTickerController> _logger;
        private readonly IStockTickerService _stockTickerService;

        public StockTickerController(
            ILogger<StockTickerController> logger,
            IStockTickerService stockTickerService)
        {
            _logger = logger;
            _stockTickerService = stockTickerService;
        }

        [HttpGet]
        [Route("GetTopTickers")]
        public async Task<IActionResult> GetTopTickers(
            [FromQuery] DateTime? startDate,
            [FromQuery] DateTime? endDate,
            [FromQuery] int page = 0)
        {
            var start = startDate ?? DateTime.Now.AddDays(-1);
            var end= endDate ?? DateTime.Now;
            var result = await _stockTickerService.GetMostMentionedTickers(start, end, page);
            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllTickers()
        {
            var result = await _stockTickerService.GetAllTickersAsync();
            return Ok(result);
        }

        [HttpPost("Batch")]
        public async Task<IActionResult> PostBatch(IEnumerable<StockTicker> stockTickers)
        {
            var results = await _stockTickerService.BulkTickerInsertAsync(stockTickers);
            return StatusCode(201, results);
        }
    }
}
