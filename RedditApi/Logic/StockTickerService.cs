using System;
using System.Collections.Generic;
using System.Data;
using Common.Models;
using RedditApi.DataAccess;

namespace RedditApi.Logic
{
    public class StockTickerService : IStockTickerService
    {
        private readonly IStockTickerRepo _stockTickerRepo;
        private readonly IDbConnection _connection;


        public StockTickerService(
            IStockTickerRepo stockTickerRepo,
            IDbConnection connection)
        {
            _stockTickerRepo = stockTickerRepo;
            _connection = connection;
        }

        public void CreateTicker(StockTicker ticker)
        {
            _connection.Open();
            try
            {
                using(var trans = _connection.BeginTransaction())
                {
                    _stockTickerRepo.CreateTickerDBAsync(ticker, _connection);
                    trans.Commit();
                }
            }
            catch (Exception ex)
            {

            }
            finally
            {
                _connection.Close();
            }
        }

        public void BulkTickerInsert(IEnumerable<StockTicker> tickers)
        {
            foreach(var ticker in tickers)
            {
                CreateTicker(ticker);
            }
        }
    }
}