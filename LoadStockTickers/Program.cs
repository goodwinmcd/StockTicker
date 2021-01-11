using System;
using System.Linq;
using System.Net;
using System.Collections.Generic;
using System.IO;
using CsvHelper;
using System.Globalization;
using CsvHelper.Configuration.Attributes;
using System.Net.Http;
using Newtonsoft.Json;
using System.Text;
using System.Net.Security;

namespace LoadStockTickers
{
    class Program
    {
        private static Dictionary<string, string> _urlList = new Dictionary<string, string>()
        {
            { "ftp://ftp.nasdaqtrader.com/SymbolDirectory/nasdaqlisted.txt", "nasdaq.csv" },
            { "ftp://ftp.nasdaqtrader.com/SymbolDirectory/otherlisted.txt", "others.csv"}
        };

        private static Dictionary<string, string> _exchangeMapper = new Dictionary<string, string>()
        {
            { "A", "NYSEMKT" },
            { "N", "NYSE" },
            { "P", "NYSEARCA" },
            { "Z", "BATS" },
            { "V", "IEXG" },
            { "nasdaq", "NASDAQ" }
        };
        static void Main(string[] args)
        {
            foreach (KeyValuePair<string, string> url in _urlList)
            {
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(url.Key);
                request.Method = WebRequestMethods.Ftp.DownloadFile;
                request.Credentials = new NetworkCredential("anonymous","janeDoe@contoso.com");
                FtpWebResponse response = (FtpWebResponse)request.GetResponse();
                Stream responseStream = response.GetResponseStream();
                using(var writer = new FileStream(url.Value, FileMode.Create))
                {
                    var length = response.ContentLength;
                    var bufferSize = 2048;
                    int readCount;
                    var buffer = new byte[2048];

                    readCount = responseStream.Read(buffer, 0, bufferSize);
                    while (readCount > 0)
                    {
                        writer.Write(buffer, 0, readCount);
                        readCount = responseStream.Read(buffer, 0, bufferSize);
                    }
                }
                Console.WriteLine($"Download Complete, status {response.StatusDescription}");
                response.Close();

            }

            var listOfNasdaqTickers = BuildList<CsvTickerNasdaqModel>("nasdaq.csv");
            var listOfOtherTickers = BuildList<CsvTickerOtherModel>("others.csv");
            var allTickers = listOfNasdaqTickers.Concat(listOfOtherTickers);

            var content = new StringContent(
                JsonConvert.SerializeObject(allTickers),
                Encoding.UTF8,
                "application/json");
            using (var httpClientHandler = new HttpClientHandler())
            {
                httpClientHandler.ServerCertificateCustomValidationCallback = (message, cert, chain, sslPolicyErrors) => {
                    if (sslPolicyErrors == SslPolicyErrors.None)
                    {
                        return true;   //Is valid
                    }

                    return true;
                };
                using (var httpClient = new HttpClient(httpClientHandler))
                {
                    var watch = System.Diagnostics.Stopwatch.StartNew();
                    httpClient.Timeout = TimeSpan.FromMinutes(30);
                    // var httpResponse = httpClient.PostAsync("http://stocktickerapi.us-west-1.elasticbeanstalk.com/stockticker/batch", content).Result;
                    var httpResponse = httpClient.PostAsync("http://localhost:5000/stockticker/batch", content).Result;
                    watch.Stop();
                    var elaspedTime = watch.ElapsedMilliseconds;
                }
            }
        }

        private static List<StockTickerDb> BuildList<T>(string file) where T : NasdaqTickerModel
        {
            // remove timestamp at end of file
            var lines = File.ReadAllLines(file);
            File.WriteAllLines(file, lines.Take(lines.Length - 1).ToArray());
            var returnList = new List<StockTickerDb>();
            using (var reader = File.OpenText(file))
            using (var csvReader = new CsvReader(reader, CultureInfo.CurrentCulture))
            {
                csvReader.Configuration.HasHeaderRecord = true;
                csvReader.Configuration.Delimiter = "|";
                while(csvReader.Read())
                {
                    var record = csvReader.GetRecord<T>();
                    _exchangeMapper.TryGetValue(record.Exchange, out string exchange);
                    returnList.Add(new StockTickerDb
                    {
                        NasdaqSymbol = record.Symbol.ToUpper(),
                        Exchange = exchange,
                        SecurityName = record.SecurityName
                    });
                }
            }
            return returnList;
        }
    }
    // ACT Symbol|Security Name|Exchange|CQS Symbol|ETF|Round Lot Size|Test Issue|NASDAQ Symbol

    public interface NasdaqTickerModel
    {
        string Symbol { get; set; }
        string Exchange { get; }
        string SecurityName { get; set; }
    }

    public class CsvTickerNasdaqModel : NasdaqTickerModel
    {
        private string _exchange = "nasdaq";
        [Index(0)]
        public string Symbol { get; set; }
        [Index(1)]
        public string SecurityName { get; set; }
        [Index(2)]
        public string LastSale { get; set; }
        [Index(3)]
        public string MarketCap { get; set; }
        [Index(4)]
        public string IPOyear { get; set; }
        [Index(5)]
        public string Sector { get; set; }
        [Index(6)]
        public string industry { get; set; }
        [Index(7)]
        public string Summary { get; set; }
        public string Exchange => _exchange;
    }

    public class CsvTickerOtherModel : NasdaqTickerModel
    {
        [Index(0)]
        public string ActSymbol { get; set; }
        [Index(1)]
        public string SecurityName { get; set; }
        [Index(2)]
        public string Exchange { get; set; }
        [Index(3)]
        public string CqsSymbol { get; set; }
        [Index(4)]
        public string Etf { get; set; }
        [Index(5)]
        public string RoundLotSize { get; set; }
        [Index(6)]
        public string TestIssue { get; set; }
        [Index(7)]
        public string Symbol { get; set; }
    }

    public class StockTickerDb
    {
        public String NasdaqSymbol { get; set; }
        public String Exchange { get; set; }
        public String SecurityName { get; set; }
    }
}
