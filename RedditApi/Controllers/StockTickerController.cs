using System;
using System.Collections.Generic;
using System.Linq;
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

        [HttpPost]
        public IActionResult Post(StockTicker ticker)
        {
            _stockTickerService.CreateTicker(ticker);
            return StatusCode(201, "Created");
        }

        [HttpPost("Batch")]
        public IActionResult PostBatch(IEnumerable<StockTicker> stockTickers)
        {
            if (stockTickers.Count() > 250)
                return StatusCode(413);
            _stockTickerService.BulkTickerInsert(stockTickers);
            return Ok();
        }
    }
}
