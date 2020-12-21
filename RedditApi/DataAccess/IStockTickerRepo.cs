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
    }
}