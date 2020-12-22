using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using Common.Models;
using Newtonsoft.Json;

namespace RedditMonitorWorker.Logic
{
    public class StockTickerManager : IStockTickerManager
    {

        public IEnumerable<string> AllStockTickers { get; set; }

        public StockTickerManager()
        {
            LoadStockTickerList();
        }

        private void LoadStockTickerList()
        {
            using (var httpClientHandler = new HttpClientHandler())
            using (var httpClient = new HttpClient(httpClientHandler))
            {
                httpClientHandler.ServerCertificateCustomValidationCallback = (message, cert, chain, sslPolicyErrors) => {
                    return true;
                };
                var result = httpClient.GetAsync("http://localhost:5000/stockticker").Result;
                var tickers =
                    JsonConvert.DeserializeObject<List<StockTicker>>(
                        result.Content.ReadAsStringAsync().Result);
                AllStockTickers = tickers.Select(t => t.NasdaqSymbol);
            }
        }
    }
}