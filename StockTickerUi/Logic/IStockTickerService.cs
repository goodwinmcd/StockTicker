using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using StockTickerUi.Models;

namespace StockTickerUi.Logic
{
    public interface IStockTickerService
    {
        Task<IEnumerable<StockTickerWithCount>> GetTopStockTickersWithCount(
            DateTime startDate, DateTime endDate, int page, string source = null);
        Task<int> GetPagingData(DateTime startDate, DateTime endDate);
    }
}