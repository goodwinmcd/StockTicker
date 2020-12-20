using System.Collections.Generic;
using Common.Models;

namespace RedditApi.Logic
{
    public interface IStockTickerService
    {
         void CreateTicker(StockTicker ticker);
         void BulkTickerInsert(IEnumerable<StockTicker> tickers);
    }
}