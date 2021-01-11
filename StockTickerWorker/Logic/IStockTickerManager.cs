using System.Collections.Generic;

namespace StockTickerWorker.Logic
{
    public interface IStockTickerManager
    {
        IEnumerable<string> FindMatchingTickers(IEnumerable<string> message);
    }
}