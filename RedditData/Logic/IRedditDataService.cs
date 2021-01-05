using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Common.Models;

namespace RedditData.Logic
{
    public interface IRedditDataService
    {
        Task<IEnumerable<StockTickerCountDb>> GetTopStockTickersWithCount(
            DateTime startDate, DateTime endDate, int page, string source = null);
        Task<int> GetPagingData(DateTime startDate, DateTime endDate);
    }
}