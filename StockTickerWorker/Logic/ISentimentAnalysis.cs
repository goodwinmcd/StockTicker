using System.Threading.Tasks;

namespace StockTickerWorker.Logic
{
    public interface ISentimentAnalysis
    {
        Task<int> GetSentimentAsIntAsync(string messageText);
    }
}