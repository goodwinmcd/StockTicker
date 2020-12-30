using System;
using System.Threading.Tasks;
using Common.Models;
using Microsoft.AspNetCore.Mvc;
using RedditData.Logic;

namespace RedditData.Controllers
{
    public class RedditDataController : Controller
    {
        private readonly IRedditDataService _redditDataService;

        public RedditDataController(IRedditDataService redditDataService)
        {
            _redditDataService = redditDataService;
        }

        public async Task<IActionResult> Index(
            DateTime? startDate, DateTime? endDate, [FromQuery]int page = 0)
        {
            var startDateValid = startDate ?? DateTime.Now.AddDays(-1);
            var endDateValid = endDate ?? DateTime.Now;
            var tickersTask = _redditDataService.GetTopStockTickersWithCount(startDateValid, endDateValid, page);
            var pagingTask = _redditDataService.GetPagingData(startDateValid, endDateValid);
            await Task.WhenAll(tickersTask, pagingTask);
            var tickers = tickersTask.Result;
            var paging = pagingTask.Result;
            return View(new StockTickerUi
                {
                    Tickers = tickers,
                    Page = page,
                    TotalPages = paging / 16
                });
        }

        public IActionResult RedirectToRobinHood([FromQuery] string ticker)
        {
            return Redirect($"https://robinhood.com/stocks/{ticker}");
        }
    }
}