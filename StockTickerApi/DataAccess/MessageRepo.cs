using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using StockTickerApi.Models;

namespace StockTickerApi.DataAccess
{
    public class MessageRepo : IMessageRepo
    {
        public async Task<int> InsertRedditMessage(FoundMessage message, IDbConnection conn)
        {
            var sql = @"INSERT INTO foundMessage(
                source,
                subreddit,
                externalid,
                timeposted,
                message,
                sentiment
            )
                VALUES (
                    @Source,
                    @SubReddit,
                    @ExternalId,
                    @TimePosted,
                    @Message,
                    @Sentiment) RETURNING id";
            if(await MessageExists(message, conn))
            {
                return -1;
            }
            else
            {
                var result = await conn.QueryAsync<int>(sql, new {
                    Source = message.Source.ToLower(),
                    SubReddit = message.SubReddit,
                    ExternalId = message.ExternalId,
                    TimePosted = message.TimePosted,
                    Message = message.Message,
                    Sentiment = message.Sentiment,
                });
                return result.Single();
            }
        }

        public async Task InsertRedditTickerMessage(FoundMessage message, int id, IDbConnection conn)
        {
            foreach (var ticker in message.Tickers)
            {
                var sql = @"INSERT INTO stocktickersfoundmessage(
                    foundmessageid,
                    stocktickerid
                )
                    VALUES(
                        @id,
                        @ticker)";
                    await conn.ExecuteAsync(sql, new {id = id, ticker = ticker.ToUpper() });
            }
        }

        private async Task<bool> MessageExists(FoundMessage message, IDbConnection conn)
        {
            var sql = @"SELECT * FROM foundMessage WHERE externalId=@ExternalId";
            var result = await conn.QueryAsync<FoundMessage>(sql, message);
            return result.Count() != 0;
        }
    }
}