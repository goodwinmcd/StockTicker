using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using StockTickerApi.Models;

namespace StockTickerApi.Logic
{
    public interface IStockTickerService
    {
        Task<IEnumerable<StockTicker>> GetAllTickersAsync();
        Task<IEnumerable<StockTicker>> BulkTickerInsertAsync(IEnumerable<StockTicker> tickers);
        Task<IEnumerable<StockTickerWithCount>> GetMostMentionedTickers(
            DateTime startDate, DateTime endDate, int page, int limit, bool getVolume=true, string source=null);
        Task<int> GetPagingInfo(DateTime startDate, DateTime endDate);
    }
}