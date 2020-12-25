using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using Common.Models;
using Newtonsoft.Json;

namespace RedditMonitorWorker.Logic
{
    public class StockTickerManager : IStockTickerManager
    {

        private IEnumerable<string> _stockTickers;
        private IEnumerable<string> _commonWordTickers;

        public StockTickerManager()
        {
            _commonWordTickers = GetCommonWordTickers();
            // exclude the common word tickers list from stock ticker list
            _stockTickers = LoadStockTickerList().Except(_commonWordTickers, StringComparer.OrdinalIgnoreCase);
        }

        public IEnumerable<string> FindMatchingTickers(IEnumerable<string> message)
        {
            // if the common word tickers are all uppercase in original message
            // then we'll assume it's a stock ticker
            var matchingCommonWordTickers = _commonWordTickers.Intersect(message);
            // match the rest of the tickers that are not common words
            var matchingAllTickers = _stockTickers.Intersect(message, StringComparer.OrdinalIgnoreCase);
            return matchingCommonWordTickers.Concat(matchingAllTickers);
        }

        private IEnumerable<string> LoadStockTickerList()
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
                return tickers.Select(t => t.NasdaqSymbol.ToLower());
            }
        }

        private IEnumerable<string> GetCommonWordTickers()
        {
            return new HashSet<string>
            {
                "WISH",
                "NICE",
                "ON",
                "A",
                "FOR",
                "SO",
                "TWO",
                "ALL",
                "ARE",
                "IT",
                "NOW",
                "JACK",
                "MA",
                "ANY",
                "BE",
                "CAN",
                "SEE",
                "OUT",
                "JUST",
                "ONE",
                "MAN",
                "OR",
                "BIT",
                "LOVE",
                "TELL",
                "GOOD",
                "AT",
                "IVE",
                "U",
                "GO",
                "EARN",
                "HOLD",
                "PUMP",
                "YOLO",
                "BIG",
                "R",
                "AM",
                "BRO",
                "HAS",
                "FIX",
                "C",
                "HE",
                "BEST",
                "FAN",
                "X",
                "K",
                "MOON",
                "POST",
                "GAIN",
                "BY",
                "SUB",
                "BLUE",
                "WELL",
                "FUN",
                "CAMP",
                "PLAY",
                "LOW",
                "VERY",
                "BILL",
                "HAS",
                "AN",
                "CEO",
                "OPEN",
                "IPO",
                "DDS",
                "Z",
                "J",
                "FILL",
                "A",
                "MAIN",
                "WANT",
                "THO",
                "LIFE",
                "NEW",

            };
        }
    }
}