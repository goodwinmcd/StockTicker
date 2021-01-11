using System;
using System.Threading.Tasks;
using Common.Models;
using Microsoft.AspNetCore.Mvc;
using RedditData.Logic;
using RedditData.Models;

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
            [FromQuery] string timeFrame,
            [FromQuery]int page = 0,
            [FromQuery] string source = null)
        {
            var parsedTimes = new TimeFrameSelection(timeFrame);
            var tickersTask = _redditDataService.GetTopStockTickersWithCount(
                parsedTimes.StartDate,
                parsedTimes.EndDate,
                page,
                source);
            var pagingTask = _redditDataService.GetPagingData(parsedTimes.StartDate, parsedTimes.EndDate);
            await Task.WhenAll(tickersTask, pagingTask);
            var tickers = tickersTask.Result;
            var paging = pagingTask.Result;
            return View(new StockTickerUi
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