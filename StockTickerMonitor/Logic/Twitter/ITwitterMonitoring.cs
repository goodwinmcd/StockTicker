using System.Threading.Tasks;

namespace StockTickerMonitor.Logic.Twitter
{
    public interface ITwitterMonitoring
    {
         Task MonitorTweetsAsync();
    }
}