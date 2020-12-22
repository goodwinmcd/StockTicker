using System.Collections.Generic;
using Common.Models;

namespace RedditMonitorWorker.Logic
{
    public interface IStockTickerManager
    {
        IEnumerable<string> AllStockTickers { get; set; }
    }
}