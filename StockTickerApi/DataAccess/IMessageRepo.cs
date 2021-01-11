using System.Data;
using System.Threading.Tasks;
using StockTickerApi.Models;

namespace StockTickerApi.DataAccess
{
    public interface IMessageRepo
    {
        Task<int> InsertRedditMessage(FoundMessage message, IDbConnection conn);
        Task InsertRedditTickerMessage(FoundMessage message, int messageId, IDbConnection conn);
    }
}