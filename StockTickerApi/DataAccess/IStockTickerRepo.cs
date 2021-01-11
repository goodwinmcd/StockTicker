using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using StockTickerApi.Models;

namespace StockTickerApi.DataAccess
{
    public interface IStockTickerRepo
    {
        Task<IEnumerable<StockTicker>> GetAllTickersAsync(IDbConnection conn);
        Task<bool> CreateTickerDBAsync(StockTicker ticker, IDbConnection conn);
        Task<StockTicker> GetStockTickerData(string ticker, IDbConnection conn);
        Task<IEnumerable<StockTickerWithCount>> GetTopMentionedTickers(
            DateTime startDate,
            DateTime endDate,
            int page,
            int limit,
            IDbConnection conn,
            string stockTicker = null,
            string source = null);
        Task<int> GetPagingInfo(IDbConnection conn, DateTime startDate, DateTime endDate);
    }
}