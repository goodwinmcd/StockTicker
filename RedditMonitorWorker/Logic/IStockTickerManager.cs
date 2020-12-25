using System.Collections.Generic;

namespace RedditMonitorWorker.Logic
{
    public interface IStockTickerManager
    {
        IEnumerable<string> FindMatchingTickers(IEnumerable<string> message);
    }
}