using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Common.Models;

namespace RedditData.Logic
{
    public interface IRedditDataService
    {
        Task<IEnumerable<StockTickerWithCount>> GetTopStockTickersWithCount(
            DateTime startDate, DateTime endDate, int page);
    }
}