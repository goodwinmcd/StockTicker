using System;
using System.Collections.Generic;
using System.Configuration;
using System.Threading.Tasks;
using System.Linq;
using Npgsql;
using StockTickerApi.DataAccess;
using StockTickerApi.Configurations;
using StockTickerApi.Models;

namespace StockTickerApi.Logic
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

        public async Task<IEnumerable<StockTickerWithCount>> GetMostMentionedTickers(
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

                        var dailyChangeInVolume = ConvertVolumeIncrease(
                            todaysCount.CountOfOccurences, previousDaysCount.CountOfOccurences);

                        ticker.DailyChangeInVolume = double.IsNaN(dailyChangeInVolume) ? 0 : dailyChangeInVolume;
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

        private async Task<StockTickerWithCount> GetDaysCount(StockTickerWithCount ticker, DateTime start, DateTime end)
        {
            var countOfTickerInDateRange = await
                _stockTickerRepo.GetTopMentionedTickers(start, end, 0, 1, _connection, ticker.NasdaqSymbol);
            return countOfTickerInDateRange.FirstOrDefault() ?? new StockTickerWithCount
                {
                    Exchange = ticker.Exchange,
                    SecurityName = ticker.SecurityName,
                    NasdaqSymbol = ticker.NasdaqSymbol,
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