
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Security;
using System.Text;
using System.Threading.Tasks;
using Common.Models;
using Newtonsoft.Json;

namespace RedditData.Logic
{
    public class RedditDataService : IRedditDataService
    {
        public async Task<IEnumerable<StockTickerCountDb>> GetTopStockTickersWithCount(
            DateTime startDate, DateTime endDate, int page, string source=null)
        {
            var url = $"http://localhost:5000/stockticker/GetTopTickers?startDate={startDate}&endDate={endDate}&page={page}";
            var sb = new StringBuilder(url);
            if (source != null)
                sb.Append($"&source={source}");
            using (var httpClientHandler = new HttpClientHandler())
            {
                httpClientHandler.ServerCertificateCustomValidationCallback = (message, cert, chain, sslPolicyErrors) => true;
                using (var httpClient = new HttpClient(httpClientHandler))
                {
                    var test = sb.ToString();
                    var httpResponse = await httpClient.GetAsync(test);
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