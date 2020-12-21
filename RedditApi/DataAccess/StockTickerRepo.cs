using System.Data;
using System.Threading.Tasks;
using Common.Models;
using Dapper;
using Npgsql;
using System.Linq;
using System.Collections.Generic;

namespace RedditApi.DataAccess
{
    public class StockTickerRepo : IStockTickerRepo
    {
        // private readonly IDbConnection _connection;

        // public StockTickerRepo(IDbConnection connection)
        // {
        //     _connection = connection;
        // }

        public async Task<bool> CreateTickerDBAsync(StockTicker ticker, IDbConnection conn)
        {
            var sql = @"INSERT INTO stockTickers
                VALUES (
                @NasdaqSymbol,
                @Exchange,
                @SecurityName)";
            try
            {
                if (await TickerExists(ticker, conn))
                {
                    return false;
                }
                else
                {
                    await conn.ExecuteAsync(sql, ticker);
                    return true;
                }
            }
            catch (PostgresException ex)
            {
                return false;
            }

        }

        public async Task<IEnumerable<StockTicker>> GetAllTickersAsync(IDbConnection conn)
        {
            var sql = "SELECT * FROM StockTickers";
            var result = await conn.QueryAsync<StockTicker>(sql);
            return result;
        }

        public async Task<bool> TickerExists(StockTicker ticker, IDbConnection conn)
        {
            var sql = "SELECT * FROM StockTickers WHERE nasdaqSymbol=@nasdaqSymbol";
            var result = await conn.QueryAsync<StockTicker>(sql, new { nasdaqSymbol = ticker.NasdaqSymbol });
            return result.Count() != 0;
        }

    }
}