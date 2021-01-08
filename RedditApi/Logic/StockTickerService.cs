using System;
using System.Collections.Generic;
using System.Configuration;
using System.Threading.Tasks;
using System.Linq;
using Common.Models;
using Npgsql;
using RedditApi.DataAccess;
using RedditMonitor.Configurations;

namespace RedditApi.Logic
{
    public class StockTickerService : IStockTickerService
    {
        private readonly IStockTickerRepo _stockTickerRepo;
        private readonly NpgsqlConnection _connection;


        public StockTickerService(
            IServiceConfigurations configurations,
            IStockTickerRepo stockTickerRepo)
        {
            _stockTickerRepo = stockTickerRepo;
            _connection = new NpgsqlConnection(configurations.dbConnectionString);
        }

        public async Task<int> GetPagingInfo(DateTime startDate, DateTime endDate)
        {
            try
            {
                await _connection.OpenAsync();
                var result = await _stockTickerRepo.GetPagingInfo(_connection, startDate, endDate);
                return result;
            }
            finally
            {
                await _connection.CloseAsync();
            }
        }

        public async Task<IEnumerable<StockTickerCountDb>> GetMostMentionedTickers(
            DateTime startDate,
            DateTime endDate,
            int page,
            int limit,
            bool getVolume=true,
            string source=null)
        {
            try
            {
                await _connection.OpenAsync();
                var countOfMentionedStockTickers =
                    await _stockTickerRepo.GetTopMentionedTickers(
                        startDate, endDate, page, limit, _connection, source: source);
                if (getVolume)
                {
                    foreach (var ticker in countOfMentionedStockTickers)
                    {
                        var previousDaysCount =
                            await GetDaysCount(ticker, DateTime.Now.AddDays(-2).ToUniversalTime(), DateTime.Now.AddDays(-1).ToUniversalTime());
                        var todaysCount =
                            await GetDaysCount(ticker, DateTime.Now.AddDays(-1).ToUniversalTime(), DateTime.Now.ToUniversalTime());

                        ticker.DailyChangeInVolume = ConvertVolumeIncrease(
                            todaysCount.CountOfOccurences, previousDaysCount.CountOfOccurences);
                    }
                }
                return countOfMentionedStockTickers;
            }
            finally
            {
                await _connection.CloseAsync();
            }
        }

        private double ConvertVolumeIncrease(double todaysCount, double yesterdaysCount)
            => Math.Round(((todaysCount - yesterdaysCount) /
                (yesterdaysCount + todaysCount)) * 100, 2);

        private async Task<StockTickerCountDb> GetDaysCount(StockTickerCountDb ticker, DateTime start, DateTime end)
        {
            var countOfTickerInDateRange = await
                _stockTickerRepo.GetTopMentionedTickers(start, end, 0, 1, _connection, ticker.NasdaqSymbol);
            return countOfTickerInDateRange.FirstOrDefault() ?? new StockTickerCountDb
                {
                    Exchange = ticker.Exchange,
                    SecurityName = ticker.SecurityName,
                    NasdaqSymbol = ticker.NasdaqSymbol,
                    CountOfOccurences = 0
                };
        }

        public async Task<IEnumerable<StockTickerDb>> BulkTickerInsertAsync(IEnumerable<StockTickerDb> tickers)
        {
            try
            {
                IEnumerable<StockTickerDb> successfulInserts = new List<StockTickerDb>();
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

        public async Task<IEnumerable<StockTickerDb>> GetAllTickersAsync()
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