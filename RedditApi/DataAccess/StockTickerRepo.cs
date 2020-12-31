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

        public async Task<IEnumerable<StockTickerCountDb>> GetTopMentionedTickers(
            DateTime startDate,
            DateTime endDate,
            int page,
            int limit,
            IDbConnection conn,
            string stockTicker = null,
            string source = null)
        {
            var sql = new StringBuilder();
            var offset = page * limit;
            sql.Append(@"SELECT st.nasdaqsymbol, st.exchange, st.securityname, COUNT(*) AS CountOfOccurences
                        FROM redditMessage AS rm
                        JOIN stocktickersredditmessage AS strm ON rm.id = strm.redditmessageid
                        JOIN stocktickers as st ON st.nasdaqsymbol = strm.stocktickerid
                        WHERE rm.timeposted > @StartDate AND rm.timeposted < @EndDate");
            if (stockTicker != null)
                sql.Append(@" AND strm.stocktickerid = @StockTicker ");
            sql.Append(@" GROUP BY st.nasdaqsymbol, st.exchange, st.securityname
                        ORDER BY CountOfOccurences desc
                        OFFSET @Offset
                        LIMIT @Limit");
            var result = await conn.QueryAsync<StockTickerCountDb>(sql.ToString(), new {
                StartDate = startDate,
                EndDate = endDate,
                Offset = offset,
                StockTicker = stockTicker,
                Limit = limit,
            });
            return result;
        }

        public async Task<int> GetPagingInfo(
            IDbConnection conn,
            DateTime startDate,
            DateTime endDate)
        {
            var sql = @"SELECT COUNT(DISTINCT strm.stocktickerid) FROM stocktickersredditmessage AS strm";
            var result = await conn.QueryAsync<PagingResultDb>(sql);
            return result.FirstOrDefault().Count;
        }

        public async Task<StockTickerDb> GetStockTickerData(string ticker, IDbConnection conn)
        {
            var sql = @"SELECT * FROM stocktickers WHERE nasdaqsymbol = @Ticker";
            var result = await conn.QueryAsync<StockTickerDb>(sql, new { Ticker = ticker });
            return result.FirstOrDefault();
        }

        public async Task<bool> CreateTickerDBAsync(StockTickerDb ticker, IDbConnection conn)
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

        public async Task<IEnumerable<StockTickerDb>> GetAllTickersAsync(IDbConnection conn)
        {
            var sql = "SELECT * FROM StockTickers";
            var result = await conn.QueryAsync<StockTickerDb>(sql);
            return result;
        }

        public async Task<bool> TickerExists(StockTickerDb ticker, IDbConnection conn)
        {
            var sql = "SELECT * FROM StockTickers WHERE nasdaqSymbol=@nasdaqSymbol";
            var result = await conn.QueryAsync<StockTickerDb>(sql, new { nasdaqSymbol = ticker.NasdaqSymbol });
            return result.Count() != 0;
        }

    }
}