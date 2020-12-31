using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Common.Models;

namespace RedditApi.Logic
{
    public interface IStockTickerService
    {
        Task<IEnumerable<StockTickerDb>> GetAllTickersAsync();
        Task<IEnumerable<StockTickerDb>> BulkTickerInsertAsync(IEnumerable<StockTickerDb> tickers);
        Task<IEnumerable<StockTickerWithCount>> GetMostMentionedTickers(
            DateTime startDate, DateTime endDate, int page, int limit, bool getVolume=true);
        Task<int> GetPagingInfo(DateTime startDate, DateTime endDate);
    }
}