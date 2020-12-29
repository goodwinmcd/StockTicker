using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Common.Models;

namespace RedditApi.DataAccess
{
    public interface IStockTickerRepo
    {
        Task<IEnumerable<StockTicker>> GetAllTickersAsync(IDbConnection conn);
        Task<bool> CreateTickerDBAsync(StockTicker ticker, IDbConnection conn);
        Task<StockTicker> GetStockTickerData(string ticker, IDbConnection conn);
        Task<IEnumerable<StockTickerCount>> GetTopMentionedTickers(
            DateTime startDate,
            DateTime endDate,
            int page,
            IDbConnection conn,
            string stockTicker = null);
    }
}