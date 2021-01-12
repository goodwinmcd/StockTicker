using System;
using System.Threading.Tasks;
using Common.RabbitMQ;
using Reddit;
using Reddit.Controllers;
using Reddit.Controllers.EventArgs;
using StockTickerMonitor.Configurations;
using StockTickerMonitor.Models;

namespace StockTickerMonitor.Logic
{
    public class RedditMonitoring : IRedditMonitoring
    {
        private readonly IRabbitPublisher _rabbitPublisher;
        private readonly String _routingKey = "messagesToProcess";
        private readonly RedditClient _redditClient;
        private readonly IServiceConfigurations _serviceConfigurations;

        public RedditMonitoring(
            IRabbitPublisher rabbitManager,
            IServiceConfigurations serviceConfigurations)
        {
            _rabbitPublisher = rabbitManager;
            _serviceConfigurations = serviceConfigurations;
            _redditClient = new RedditClient(
                _serviceConfigurations.RedditAppId,
                _serviceConfigurations.RedditOAuthKey);
        }

        public async Task MonitorPostsAsync()
        {
            await Task.Factory.StartNew(() => {
                foreach (var subreddit in Enum.GetValues(typeof(SubReddit)))
                {
                    var newSubredditClient = _redditClient.Subreddit(subreddit.ToString());
                    newSubredditClient.Comments.GetNew();
                    newSubredditClient.Comments.MonitorNew();
                    newSubredditClient.Comments.NewUpdated += C_AddNewCommentToQueue;
                    newSubredditClient.Posts.GetNew();
                    newSubredditClient.Posts.MonitorNew();
                    newSubredditClient.Posts.NewUpdated += C_AddNewPostToQueue;
                }
            });
        }

        private void C_AddNewCommentToQueue(object sender, CommentsUpdateEventArgs eventArgs)
        {
            foreach (var comment in eventArgs.Added)
            {
                var payload = BuildRedditQueueMessageFromRedditMessage(new CommentWrapper(comment));
                _rabbitPublisher.Publish<QueueMessage>(payload, _routingKey);

            }
            var dept = _serviceConfigurations.QueueHost;
        }

        private void C_AddNewPostToQueue(object sender, PostsUpdateEventArgs eventArgs)
        {
            foreach (var post in eventArgs.Added)
            {
                var payload = BuildRedditQueueMessageFromRedditMessage(new PostWrapper(post));
                _rabbitPublisher.Publish<QueueMessage>(payload, _routingKey);

            }
            var dept = _serviceConfigurations.QueueHost;
        }

        private QueueMessage BuildRedditQueueMessageFromRedditMessage(IRedditWrappers message)
        {
            if (!Enum.TryParse(message.SubredditWrap, true, out SubReddit subReddit))
                Console.WriteLine($"Unable to parse subreddit: {message.SubredditWrap}");
            return new QueueMessage
                {

                    TraceId = Guid.NewGuid(),
                    MessageContent = new FoundMessage {
                            Source = message.Source,
                            SubReddit = subReddit.ToString(),
                            ExternalId = message.RedditId,
                            TimePosted = message.CreatedWrap,
                            Message = message.Content,
                        },
                };

        }

        private interface IRedditWrappers
        {
            public String Source { get; }
            public string RedditId { get; }
            public DateTime CreatedWrap { get; }
            public string Content { get; }
            public string SubredditWrap { get; }
        }

        private class PostWrapper : IRedditWrappers
        {
            private Post _base;
            public PostWrapper(Post basePost)
            {
                _base = basePost;
            }
            public String Source { get => MessageSource.Reddit.ToString(); }
            public string RedditId { get => _base.Id; }
            public DateTime CreatedWrap { get => _base.Created; }
            public string Content { get => _base.Title; }
            public string SubredditWrap { get => _base.Subreddit; }
        }

        private class CommentWrapper : IRedditWrappers
        {
            private Comment _base;
            public CommentWrapper(Comment baseComment)
            {
                _base = baseComment;
            }
            public String Source { get => MessageSource.Reddit.ToString(); }
            public string RedditId { get => _base.Id; }
            public DateTime CreatedWrap { get => _base.Created; }
            public string Content { get =>  _base.Body; }
            public string SubredditWrap { get => _base.Subreddit; }
        }
    }
}