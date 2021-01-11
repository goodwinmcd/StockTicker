using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using Newtonsoft.Json;
using StockTickerWorker.Models;
using StockTickerWorker.ServiceConfiguration;

namespace StockTickerWorker.Logic
{
    public class StockTickerManager : IStockTickerManager
    {

        private IEnumerable<string> _stockTickers;
        private IEnumerable<string> _commonWordTickers;
        private readonly IServiceConfigurations _serviceConfigurations;

        public StockTickerManager(IServiceConfigurations serviceConfigurations)
        {
            _serviceConfigurations = serviceConfigurations;
            _commonWordTickers = ListOfCommonTickers.CommonTickerNames;
            // exclude the common word tickers list from stock ticker list
            _stockTickers = LoadStockTickerList().Except(_commonWordTickers, StringComparer.OrdinalIgnoreCase);
            _stockTickers = _stockTickers.Where(x => !TickersToCompletelyIgnore().Contains(x));
        }

        public IEnumerable<string> FindMatchingTickers(IEnumerable<string> message)
        {
            // if the common word tickers are all uppercase in original message
            // then we'll assume it's a stock ticker
            var matchingCommonWordTickers = _commonWordTickers.Intersect(message);
            // match the rest of the tickers that are not common words
            var matchingAllTickers = _stockTickers.Intersect(message, StringComparer.OrdinalIgnoreCase);
            return matchingCommonWordTickers.Concat(matchingAllTickers).Where(x => x != "A" || x != "a");
        }

        private IEnumerable<string> LoadStockTickerList()
        {
            using (var httpClientHandler = new HttpClientHandler())
            using (var httpClient = new HttpClient(httpClientHandler))
            {
                httpClientHandler.ServerCertificateCustomValidationCallback = (message, cert, chain, sslPolicyErrors) => {
                    return true;
                };
                var result = httpClient.GetAsync($"{_serviceConfigurations.ApiUrl}/stockticker").Result;
                var tickers =
                    JsonConvert.DeserializeObject<List<StockTicker>>(
                        result.Content.ReadAsStringAsync().Result);
                return tickers.Select(t => t.NasdaqSymbol.ToLower());
            }
        }

        private IEnumerable<string> TickersToCompletelyIgnore()
            => new List<string>
            {
                "YOLO",
                "EV",
                "RH",
                "C",
                "F",
                "A",
                "DD",
                "K",
                "D",
            };
    }
}