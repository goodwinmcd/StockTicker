using System.Data;
using System.Threading.Tasks;
using Common.Models;
using Dapper;
using System.Linq;
using System.Collections.Generic;
using System;
using System.Text;

namespace RedditApi.DataAccess
{
    public class StockTickerRepo : IStockTickerRepo
    {

        public async Task<IEnumerable<StockTickerCount>> GetTopMentionedTickers(
            DateTime startDate,
            DateTime endDate,
            int page,
            IDbConnection conn,
            string stockTicker = null)
        {
            var sql = new StringBuilder();
            var offset = page * 16;
            sql.Append(@"SELECT strm.stocktickerid, COUNT(*) AS CountOfOccurences
                        FROM redditMessage AS rm
                        JOIN stocktickersredditmessage AS strm ON rm.id = strm.redditmessageid
                        JOIN stocktickers as st ON st.nasdaqsymbol = strm.stocktickerid
                        WHERE rm.timeposted > @StartDate AND rm.timeposted < @EndDate");
            if (stockTicker != null)
                sql.Append(@" AND strm.stocktickerid = @StockTicker ");
            sql.Append(@" GROUP BY strm.stocktickerid
                        ORDER BY CountOfOccurences desc
                        OFFSET @Offset
                        LIMIT 16");
            var result = await conn.QueryAsync<StockTickerCount>(sql.ToString(), new {
                StartDate = startDate,
                EndDate = endDate,
                Offset = offset,
                StockTicker = stockTicker,
            });
            return result;
        }

        public async Task<int> GetPagingInfo(
            IDbConnection conn,
            DateTime startDate,
            DateTime endDate)
        {
            var sql = @"SELECT COUNT(DISTINCT strm.stocktickerid) FROM stocktickersredditmessage AS strm";
            var result = await conn.QueryAsync(sql);
            return result.FirstOrDefault();
        }

        public async Task<StockTicker> GetStockTickerData(string ticker, IDbConnection conn)
        {
            var sql = @"SELECT * FROM stocktickers WHERE nasdaqsymbol = @Ticker";
            var result = await conn.QueryAsync<StockTicker>(sql, new { Ticker = ticker });
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