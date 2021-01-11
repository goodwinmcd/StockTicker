using System.Threading.Tasks;

namespace StockTickerMonitor.Logic
{
    public interface IRedditMonitoring
    {
         Task MonitorPostsAsync();
    }
}