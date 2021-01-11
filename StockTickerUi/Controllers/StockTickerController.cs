using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using StockTickerUi.Logic;
using StockTickerUi.Models;

namespace StockTickerUi.Controllers
{
    public class StockTickerController : Controller
    {
        private readonly IStockTickerService _stockTickerService;

        public StockTickerController(IStockTickerService stockTickerService)
        {
            _stockTickerService = stockTickerService;
        }

        public async Task<IActionResult> Index(
            [FromQuery] string timeFrame,
            [FromQuery]int page = 0,
            [FromQuery] string source = null)
        {
            var parsedTimes = new TimeFrameSelection(timeFrame);
            var tickersTask = _stockTickerService.GetTopStockTickersWithCount(
                parsedTimes.StartDate,
                parsedTimes.EndDate,
                page,
                source);
            var pagingTask = _stockTickerService.GetPagingData(parsedTimes.StartDate, parsedTimes.EndDate);
            await Task.WhenAll(tickersTask, pagingTask);
            var tickers = tickersTask.Result;
            var paging = pagingTask.Result;
            return View(new StockTickerView
                {
                    Tickers = tickers,
                    Page = page,
                    TotalPages = paging / 16,
                    Source = source ?? "All",
                    SelectedDateRange = timeFrame,
                });
        }

        public IActionResult RedirectToRobinHood([FromQuery] string ticker)
        {
            return Redirect($"https://robinhood.com/stocks/{ticker}");
        }
    }
}