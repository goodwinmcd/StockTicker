using System;
using System.Collections.Generic;
using System.Linq;
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
            DateTime? startDate, DateTime? endDate, string stockTicker = null)
        {
            var startDateValid = startDate ?? DateTime.Now.AddDays(-1);
            var endDateValid = endDate ?? DateTime.Now;
            var tickersTask = _redditDataService.GetTopStockTickersWithCount(startDateValid, endDateValid, 0);
            var pagingTask = _redditDataService.GetPagingData(startDateValid, endDateValid);
            var results = Task.WhenAll(tickersTask, pagingTask);
            var tickers = tickersTask.Result;
            var paging = pagingTask.Result;
            var tickersForView = tickers.Select(t => new StockTickerUi(t, paging));
            return View(tickersForView);
        }
    }
}