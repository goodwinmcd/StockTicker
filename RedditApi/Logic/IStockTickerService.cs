using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Common.Models;

namespace RedditApi.Logic
{
    public interface IStockTickerService
    {
        Task<IEnumerable<StockTicker>> GetAllTickersAsync();
        Task<IEnumerable<StockTicker>> BulkTickerInsertAsync(IEnumerable<StockTicker> tickers);
        Task<IEnumerable<StockTickerCountUi>> GetMostMentionedTickers(
            DateTime startDate, DateTime endDate, int page);
        Task<int> GetPagingInfo(DateTime startDate, DateTime endDate);
    }
}