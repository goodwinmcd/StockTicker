using System.Data;
using System.Threading.Tasks;
using Dapper;
using System.Linq;
using System.Collections.Generic;
using System;
using System.Text;
using StockTickerApi.Models;

namespace StockTickerApi.DataAccess
{
    public class StockTickerRepo : IStockTickerRepo
    {

        public async Task<IEnumerable<StockTickerWithCount>> GetTopMentionedTickers(
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
                        FROM foundMessage AS fm
                        JOIN stocktickersfoundmessage AS stfm ON fm.id = stfm.foundmessageid
                        JOIN stocktickers as st ON st.nasdaqsymbol = stfm.stocktickerid
                        WHERE fm.timeposted > @StartDate AND fm.timeposted < @EndDate");
            if (stockTicker != null)
                sql.Append(@" AND stfm.stocktickerid = @StockTicker ");
            if (source != null)
                sql.Append(@" AND fm.source = @Source ");
            sql.Append(@" GROUP BY st.nasdaqsymbol, st.exchange, st.securityname
                        ORDER BY CountOfOccurences desc
                        OFFSET @Offset
                        LIMIT @Limit");
            var result = await conn.QueryAsync<StockTickerWithCount>(sql.ToString(), new {
                StartDate = startDate,
                EndDate = endDate,
                Offset = offset,
                StockTicker = stockTicker,
                Limit = limit,
                Source = source,
            });
            return result;
        }

        public async Task<int> GetPagingInfo(
            IDbConnection conn,
            DateTime startDate,
            DateTime endDate)
        {
            var sql = @"SELECT COUNT(DISTINCT strm.stocktickerid) FROM stocktickersfoundmessage AS strm";
            var result = await conn.QueryAsync<PagingResultDb>(sql);
            return result.FirstOrDefault().Count;
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