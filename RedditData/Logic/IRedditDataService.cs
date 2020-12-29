using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Common.Models;

namespace RedditData.Logic
{
    public interface IRedditDataService
    {
        Task<IEnumerable<StockTickerCountUi>> GetTopStockTickersWithCount(
            DateTime startDate, DateTime endDate, int page);
    }
}