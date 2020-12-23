using System.Data;
using System.Threading.Tasks;
using Common.Models;

namespace RedditApi.DataAccess
{
    public interface IRedditMessageRepo
    {
        Task<int> InsertRedditMessage(RedditMessage message, IDbConnection conn);
        Task<bool> InsertRedditTickerMessage(RedditMessage message, int messageId, IDbConnection conn);
    }
}