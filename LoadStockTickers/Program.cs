using System;
using System.Linq;
using System.Net;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Common.Models;
using CsvHelper;
using System.IO.Compression;
using System.Globalization;
using CsvHelper.Configuration.Attributes;
using System.Net.Http;
using Newtonsoft.Json;
using System.Text;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;

namespace LoadStockTickers
{
    class Program
    {
        private static Dictionary<string, string> urlList = new Dictionary<string, string>()
        {
            { "ftp://ftp.nasdaqtrader.com/SymbolDirectory/nasdaqlisted.txt", "nasdaq.csv" },
            { "ftp://ftp.nasdaqtrader.com/SymbolDirectory/otherlisted.txt", "others.csv"}
        };
        static void Main(string[] args)
        {
            foreach (KeyValuePair<string, string> url in urlList)
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

            var listOfNasdaqTickers = BuildList("nasdaq.csv");
            var listOfOtherTickers = BuildList("others.csv");
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
                    var httpResponse = httpClient.PostAsync("http://localhost:5000/stockticker/batch", content).Result;
                    watch.Stop();
                    var elaspedTime = watch.ElapsedMilliseconds;
                }
            }
        }

        private static List<StockTicker> BuildList(string file)
        {
            // remove timestamp at end of file
            var lines = File.ReadAllLines(file);
            File.WriteAllLines(file, lines.Take(lines.Length - 1).ToArray());
            var returnList = new List<StockTicker>();
            using (var reader = File.OpenText(file))
            using (var csvReader = new CsvReader(reader, CultureInfo.CurrentCulture))
            {
                csvReader.Configuration.HasHeaderRecord = true;
                csvReader.Configuration.Delimiter = "|";
                while(csvReader.Read())
                {
                    var record = csvReader.GetRecord<CsvTickerNasdaqModel>();
                    returnList.Add(new StockTicker
                    {
                        NasdaqSymbol = record.Symbol,
                        Exchange = "nasdaq",
                        SecurityName = record.Name
                    });
                }
            }
            return returnList;
        }
    }
    // ACT Symbol|Security Name|Exchange|CQS Symbol|ETF|Round Lot Size|Test Issue|NASDAQ Symbol

    public class CsvTickerNasdaqModel
    {
        [Index(0)]
        public string Symbol { get; set; }
        [Index(1)]
        public string Name { get; set; }
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
    }

    public class CsvTickerOtherModel
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
        public string NasdaqSymbol { get; set; }
    }
}
