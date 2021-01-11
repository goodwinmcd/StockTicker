using System.Threading.Tasks;
using StockTickerApi.Models;

namespace StockTickerApi.Logic
{
    public interface IMessageService
    {
        Task<int> InsertMessage(FoundMessage message);
    }
}