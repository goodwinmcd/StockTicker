using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Common.Models;

namespace RedditApi.DataAccess
{
    public interface IStockTickerRepo
    {
        Task<IEnumerable<StockTickerDb>> GetAllTickersAsync(IDbConnection conn);
        Task<bool> CreateTickerDBAsync(StockTickerDb ticker, IDbConnection conn);
        Task<StockTickerDb> GetStockTickerData(string ticker, IDbConnection conn);
        Task<IEnumerable<StockTickerCountDb>> GetTopMentionedTickers(
            DateTime startDate,
            DateTime endDate,
            int page,
            int limit,
            IDbConnection conn,
            string stockTicker = null);
        Task<int> GetPagingInfo(IDbConnection conn, DateTime startDate, DateTime endDate);
    }
}