using System;
using System.Collections.Generic;
using System.Configuration;
using System.Threading.Tasks;
using System.Linq;
using Common.Models;
using Npgsql;
using RedditApi.DataAccess;

namespace RedditApi.Logic
{
    public class StockTickerService : IStockTickerService
    {
        private readonly IStockTickerRepo _stockTickerRepo;
        private readonly NpgsqlConnection _connection;


        public StockTickerService(IStockTickerRepo stockTickerRepo)
        {
            _stockTickerRepo = stockTickerRepo;
            _connection = new NpgsqlConnection(ConfigurationManager.ConnectionStrings["pgsql"].ToString());
        }

        public async Task<IEnumerable<StockTickerCountUi>> GetMostMentionedTickers(
            DateTime startDate,
            DateTime endDate,
            int page)
        {
            try
            {
                var mostMentionedTickerData = new List<StockTickerCountUi>();
                await _connection.OpenAsync();
                var countOfMentionedStockTickers =
                    await _stockTickerRepo.GetTopMentionedTickers(
                        startDate, endDate, page, _connection);
                foreach (var ticker in countOfMentionedStockTickers)
                {
                    var previousDaysCount =
                        await GetDaysCount(ticker, DateTime.Now.AddDays(-2), DateTime.Now.AddDays(-1));
                    var todaysCount =
                        await GetDaysCount(ticker, DateTime.Now.AddDays(-3), DateTime.Now.AddDays(-2));
                    var temp = await _stockTickerRepo.GetStockTickerData(ticker.StockTickerId, _connection);
                    var volumeIncrease = ConvertVolumeIncrease(
                        todaysCount.CountOfOccurences, previousDaysCount.CountOfOccurences);
                    mostMentionedTickerData.Add(new StockTickerCountUi
                    {
                        CountOfOccurences = ticker.CountOfOccurences,
                        Exchange = temp.Exchange,
                        SecurityName = temp.SecurityName,
                        StockTickerId = temp.NasdaqSymbol,
                        VolumeIncrease = volumeIncrease
                    });
                }
                return mostMentionedTickerData;
            }
            finally
            {
                await _connection.CloseAsync();
            }
        }

        private double ConvertVolumeIncrease(double todaysCount, double yesterdaysCount)
            => Math.Round(((todaysCount - yesterdaysCount) /
                (yesterdaysCount + todaysCount)) * 100, 2);

        private async Task<StockTickerCount> GetDaysCount(StockTickerCount ticker, DateTime start, DateTime end)
        {
            var countOfTickerInDateRange = await _stockTickerRepo.GetTopMentionedTickers(
                            start,
                            end,
                            0,
                            _connection,
                            ticker.StockTickerId);
            return countOfTickerInDateRange.FirstOrDefault() ?? new StockTickerCount
                        {
                            StockTickerId = ticker.StockTickerId,
                            CountOfOccurences = 0
                        };
        }

        public async Task<IEnumerable<StockTicker>> BulkTickerInsertAsync(IEnumerable<StockTicker> tickers)
        {
            try
            {
                IEnumerable<StockTicker> successfulInserts = new List<StockTicker>();
                await _connection.OpenAsync();
                foreach(var ticker in tickers)
                    if(await _stockTickerRepo.CreateTickerDBAsync(ticker, _connection))
                        successfulInserts.Append(ticker);
                return successfulInserts;
            }
            finally
            {
                await _connection.CloseAsync();
            }
        }

        public async Task<IEnumerable<StockTicker>> GetAllTickersAsync()
        {
            try
            {
                await _connection.OpenAsync();
                var result = await _stockTickerRepo.GetAllTickersAsync(_connection);
                return result;
            }
            finally
            {
                await _connection.CloseAsync();
            }
        }
    }
}