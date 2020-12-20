using System.Data;
using System.Threading.Tasks;
using Common.Models;

namespace RedditApi.DataAccess
{
    public interface IStockTickerRepo
    {
        void CreateTickerDBAsync(StockTicker ticker, IDbConnection conn);
    }
}