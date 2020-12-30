using System.Data;
using System.Threading.Tasks;
using Common.Models;

namespace RedditApi.DataAccess
{
    public interface IRedditMessageRepo
    {
        Task<int> InsertRedditMessage(FoundMessage message, IDbConnection conn);
        Task InsertRedditTickerMessage(FoundMessage message, int messageId, IDbConnection conn);
    }
}