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
        Task<IEnumerable<StockTickerCountDb>> GetMostMentionedTickers(
            DateTime startDate, DateTime endDate, int page, int limit, bool getVolume=true, string source=null);
        Task<int> GetPagingInfo(DateTime startDate, DateTime endDate);
    }
}