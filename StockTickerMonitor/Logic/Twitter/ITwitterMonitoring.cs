using System.Threading.Tasks;

namespace RedditMonitor.Logic.Twitter
{
    public interface ITwitterMonitoring
    {
         Task MonitorTweetsAsync();
    }
}