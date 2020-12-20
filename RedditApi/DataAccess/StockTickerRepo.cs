using System.Data;
using System.Threading.Tasks;
using Common.Models;
using Dapper;
using Npgsql;

namespace RedditApi.DataAccess
{
    public class StockTickerRepo : IStockTickerRepo
    {
        // private readonly IDbConnection _connection;

        // public StockTickerRepo(IDbConnection connection)
        // {
        //     _connection = connection;
        // }

        public void CreateTickerDBAsync(StockTicker ticker, IDbConnection conn)
        {
            var sql = @"INSERT INTO stockTickers
                VALUES (
                @Ticker,
                @CompanyName,
                @Sector,
                @Industry)";
            try
            {
                conn.Execute(sql, ticker);
                // Task.Factory.StartNew(() => conn.Execute(sql));
            }
            catch (PostgresException ex)
            {
            }

        }

    }
}