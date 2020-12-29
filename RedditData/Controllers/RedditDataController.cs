using System;
using System.Threading.Tasks;
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
            var tickers = await _redditDataService.GetTopStockTickersWithCount(
                startDateValid, endDateValid, 0);
            return View(tickers);
        }
    }
}