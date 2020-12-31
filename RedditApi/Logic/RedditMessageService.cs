using System;
using System.Configuration;
using System.Threading.Tasks;
using Common.Models;
using Npgsql;
using RedditApi.DataAccess;

namespace RedditApi.Logic
{
    public class RedditMessageService : IRedditMessageService
    {
        private readonly NpgsqlConnection _connection;
        private readonly IRedditMessageRepo _commentsRepo;

        public RedditMessageService(IRedditMessageRepo commentsRepo)
        {
            _commentsRepo = commentsRepo;
            _connection = new NpgsqlConnection(
                ConfigurationManager.ConnectionStrings["pgsql"].ToString());
        }

        public async Task<int> InsertRedditMessage(FoundMessage message)
        {
            try
            {
                message.TimePosted = message.TimePosted.ToUniversalTime();
                await _connection.OpenAsync();
                using(var transaction = await _connection.BeginTransactionAsync())
                {
                    var inserted = await _commentsRepo.InsertRedditMessage(message, _connection);
                    if (inserted != -1)
                    {
                        try
                        {
                            await _commentsRepo.InsertRedditTickerMessage(message, inserted, _connection);
                            await transaction.CommitAsync();
                            return inserted;
                        }
                        catch (Exception ex)
                        {
                            await transaction.RollbackAsync();
                            throw ex;
                        }
                    }
                    else
                    {
                        return -1;
                    }
                }
            }
            finally
            {
                await _connection.CloseAsync();
            }
        }
    }
}