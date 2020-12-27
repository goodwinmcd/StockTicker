using System;
using System.Collections.Generic;
using System.Configuration;
using System.Threading.Tasks;
using System.Linq;
using Common.Models;
using Npgsql;
using RedditApi.DataAccess;
using System.Data;

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
                    await _stockTickerRepo.GetTopMentionedTickers(startDate, endDate, page, _connection);
                foreach (var ticker in countOfMentionedStockTickers)
                {
                    var temp = await _stockTickerRepo.GetStockTickerData(ticker.StockTickerId, _connection);
                    mostMentionedTickerData.Add(new StockTickerCountUi
                    {
                        CountOfOccurences = ticker.CountOfOccurences,
                        Exchange = temp.Exchange,
                        SecurityName = temp.SecurityName,
                        StockTickerId = temp.NasdaqSymbol
                    });
                }
                return mostMentionedTickerData;
            }
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