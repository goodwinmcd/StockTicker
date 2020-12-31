
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Security;
using System.Threading.Tasks;
using Common.Models;
using Newtonsoft.Json;

namespace RedditData.Logic
{
    public class RedditDataService : IRedditDataService
    {
        public async Task<IEnumerable<StockTickerCountDb>> GetTopStockTickersWithCount(
            DateTime startDate, DateTime endDate, int page)
        {
            var url = $"http://localhost:5000/stockticker/GetTopTickers?startDate={startDate}&endDate={endDate}&page={page}";
            using (var httpClientHandler = new HttpClientHandler())
            {
                httpClientHandler.ServerCertificateCustomValidationCallback = (message, cert, chain, sslPolicyErrors) => true;
                using (var httpClient = new HttpClient(httpClientHandler))
                {
                    var httpResponse = await httpClient.GetAsync(url);
                    var check =
                        JsonConvert.DeserializeObject<IEnumerable<StockTickerCountDb>>(await httpResponse.Content.ReadAsStringAsync());
                    return check;
                }
            }
        }

        public async Task<int> GetPagingData(DateTime startDate, DateTime endDate)
        {
            var url = $"http://localhost:5000/stockticker/GetPagingInfo";
            using (var httpClientHandler = new HttpClientHandler())
            {
                httpClientHandler.ServerCertificateCustomValidationCallback = (message, cert, chain, sslPolicyErrors) => true;
                using (var httpClient = new HttpClient(httpClientHandler))
                {
                    var httpResponse = await httpClient.GetAsync(url);
                    var pagingCount =
                        JsonConvert.DeserializeObject<int>(await httpResponse.Content.ReadAsStringAsync());
                    return pagingCount;
                }
            }
        }
    }
}