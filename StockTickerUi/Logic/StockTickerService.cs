
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using StockTickerUi.Configurations;
using StockTickerUi.Models;

namespace StockTickerUi.Logic
{
    public class StockTickerService : IStockTickerService
    {
        private readonly IServiceConfigurations _configurations;

        public StockTickerService(IServiceConfigurations configurations)
        {
            _configurations = configurations;
        }

        public async Task<IEnumerable<StockTickerWithCount>> GetTopStockTickersWithCount(
            DateTime startDate, DateTime endDate, int page, string source=null)
        {
            var url = $"{_configurations.ApiUrl}/stockticker/GetTopTickers?startDate={startDate}&endDate={endDate}&page={page}";
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
                        JsonConvert.DeserializeObject<IEnumerable<StockTickerWithCount>>(await httpResponse.Content.ReadAsStringAsync());
                    return check;
                }
            }
        }

        public async Task<int> GetPagingData(DateTime startDate, DateTime endDate)
        {
            var url = $"{_configurations.ApiUrl}/stockticker/GetPagingInfo";
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