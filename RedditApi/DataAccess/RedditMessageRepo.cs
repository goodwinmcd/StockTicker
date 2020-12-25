using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Common.Models;
using Dapper;
using NpgsqlTypes;

namespace RedditApi.DataAccess
{
    public class RedditMessageRepo : IRedditMessageRepo
    {
        public async Task<int> InsertRedditMessage(RedditMessage message, IDbConnection conn)
        {
            var sql = @"INSERT INTO redditMessage
                VALUES (
                    @Source,
                    @SubReddit,
                    @RedditId,
                    TO_TIMESTAMP(@TimePosted, YYYY/MM/DD HH:MM:SS),
                    @Message);
                SELECT CAST(SCOPE_IDENTITY() as int)";
            if(await MessageExists(message, conn))
            {
                return -1;
            }
            else
            {
                var test = message.TimePosted.ToString();
                var result = await conn.QueryAsync(sql, new {
                    Source = message.Source,
                    SubReddit = message.SubReddit,
                    RedditId = message.RedditId,
                    TimePosted = message.TimePosted,
                    Message = message.Message
                });
                return result.Single();
            }
        }

        public async Task InsertRedditTickerMessage(RedditMessage message, int id, IDbConnection conn)
        {
            var listOfInserts = new List<Task>();
            foreach (var ticker in message.Tickers)
            {
                var sql = @"INSERT INTO stockTickersRedditMessage
                    VALUES(
                        @id,
                        @ticker)";
                    listOfInserts.Add(conn.ExecuteAsync(sql, new {id = id, ticker = ticker }));
            }
            await Task.WhenAll(listOfInserts);
        }

        private async Task<bool> MessageExists(RedditMessage message, IDbConnection conn)
        {
            var sql = @"SELECT * FROM redditMessage WHERE redditId=@RedditId";
            var result = await conn.QueryAsync<RedditMessage>(sql, message);
            return result.Count() != 0;
        }
    }
}