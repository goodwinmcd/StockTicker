using System.Threading.Tasks;
using Dapper;
using Npgsql;

namespace RedditApi.DataAccess
{
    public class InitializeDb
    {
        private  NpgsqlConnection _stockTickerConnection;
        private  NpgsqlConnection _adminConnection;

        public InitializeDb(string stockTickerConnectionString, string adminConnectionString)
        {
            _stockTickerConnection = new NpgsqlConnection(stockTickerConnectionString);
            _adminConnection = new NpgsqlConnection(adminConnectionString);
        }

        public async Task InitializeDbAndTablesAsync()
        {
            await CreateDbAsync();
            await CreateTablesAsync();
        }

        private async Task CreateDbAsync()
        {
            try
            {
                await _adminConnection.OpenAsync();
                var sql = @"CREATE DATABASE stockTickers";
                await _adminConnection.ExecuteAsync(sql);
            }
            catch
            {
                // do nothing. I expect it to exists most of the time
            }
            finally
            {
                await _adminConnection.CloseAsync();
            }
        }

        private async Task CreateTablesAsync()
        {
            await CreateStockTickersTableAsync();
            await CreateRedditMessageTableAsync();
            await CreateStockTickersRedditMessageTableAsync();
        }

        private async Task CreateStockTickersTableAsync()
        {
            var sql = @"CREATE TABLE IF NOT EXISTS stocktickers (
                nasdaqSymbol TEXT PRIMARY KEY,
                Exchange TEXT,
                SecurityName Text
            );";
            await ExecuteQueryAsync(sql);
        }

        private async Task CreateRedditMessageTableAsync()
        {
            var sql = @"CREATE TABLE IF NOT EXISTS RedditMessage (
                id SERIAL PRIMARY KEY,
                source TEXT,
                subreddit TEXT,
                redditId TEXT,
                timePosted TimeStamp,
                message TEXT
            );";
            await ExecuteQueryAsync(sql);
        }

        private async Task CreateStockTickersRedditMessageTableAsync()
        {
            var sql = @"CREATE TABLE IF NOT EXISTS stockTickersRedditMessage(
                    redditMessageId INT NOT NULL,
                    stockTickerId TEXT NOT NULL,
                    PRIMARY KEY (redditMessageId, stockTickerId),
                FOREIGN KEY (redditMessageId)
                    REFERENCES RedditMessage(id),
                FOREIGN KEY (stockTickerId)
                    REFERENCES StockTickers (nasdaqSymbol)
                );";
            await ExecuteQueryAsync(sql);
        }

        private async Task ExecuteQueryAsync(string sql)
        {
            await _stockTickerConnection.OpenAsync();
            await _stockTickerConnection.ExecuteAsync(sql);
            await _stockTickerConnection.CloseAsync();
        }
    }
}