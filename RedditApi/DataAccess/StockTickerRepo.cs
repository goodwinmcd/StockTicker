using System.Data;
using System.Threading.Tasks;
using Common.Models;
using Dapper;
using Npgsql;
using System.Linq;
using System.Collections.Generic;
using System;

namespace RedditApi.DataAccess
{
    public class StockTickerRepo : IStockTickerRepo
    {

        public async Task<IEnumerable<StockTickerCount>> GetTopMentionedTickers(
            DateTime startDate,
            DateTime endDate,
            int page,
            IDbConnection conn)
        {
            var offset = page * 50;
            var sql = @"Select stocktickerid, COUNT(*)
                FROM stocktickersredditmessage GROUP BY stocktickerid
                BETWEEN @StartDate AND @EndDate
                OFFSET @Offset
                ORDER BY count desc";
            var result = await conn.QueryAsync<StockTickerCount>(sql);
            return result;
        }

        public async Task<StockTicker> GetStockTickerData(string ticker, IDbConnection conn)
        {
            var sql = @"SELECT * FROM stocktickers WHERE nasdaqsymbol = @Ticker";
            var result = await conn.QueryAsync<StockTicker>(sql);
            return result.FirstOrDefault();
        }

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
            catch
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