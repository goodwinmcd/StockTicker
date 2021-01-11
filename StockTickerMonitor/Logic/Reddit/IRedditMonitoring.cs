using System.Threading.Tasks;

namespace RedditMonitor.Logic
{
    public interface IRedditMonitoring
    {
         Task MonitorPostsAsync();
    }
}