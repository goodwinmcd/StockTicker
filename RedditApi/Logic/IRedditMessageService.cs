using System.Threading.Tasks;
using Common.Models;

namespace RedditApi.Logic
{
    public interface IRedditMessageService
    {
        Task<int> InsertRedditMessage(FoundMessage message);
    }
}