using System;
using System.Configuration;
using System.Threading.Tasks;
using Common.Models;
using Common.RabbitMQ;
using Reddit;
using Reddit.Controllers;
using Reddit.Controllers.EventArgs;

namespace RedditMonitor.Logic
{
    public class RedditMonitoring : IRedditMonitoring
    {
        private readonly IRabbitPublisher _rabbitPublisher;
        private readonly String _redditAppId = ConfigurationManager.AppSettings["reddit_app_id"];
        private readonly String _redditOauthKey = ConfigurationManager.AppSettings["reddit_oauth_key"];
        private readonly String _routingKey = "reddit-comments";
        private readonly RedditClient _redditClient;

        public RedditMonitoring(IRabbitPublisher rabbitManager)
        {
            _rabbitPublisher = rabbitManager;
            _redditClient = new RedditClient(_redditAppId, _redditOauthKey);
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
            var dept = ConfigurationManager.AppSettings["rabbitmqHost"];
        }

        private void C_AddNewPostToQueue(object sender, PostsUpdateEventArgs eventArgs)
        {
            foreach (var post in eventArgs.Added)
            {
                var payload = BuildRedditQueueMessageFromRedditMessage(new PostWrapper(post));
                _rabbitPublisher.Publish<QueueMessage>(payload, _routingKey);

            }
            var dept = ConfigurationManager.AppSettings["rabbitmqHost"];
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
                            RedditId = message.RedditId,
                            TimePosted = message.CreatedWrap,
                            Message = message.Content,
                        },
                };

        }

        private interface IRedditWrappers
        {
            public MessageSource Source { get; }
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
            public MessageSource Source { get => MessageSource.Reddit; }
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
            public MessageSource Source { get => MessageSource.Reddit; }
            public string RedditId { get => _base.Id; }
            public DateTime CreatedWrap { get => _base.Created; }
            public string Content { get =>  _base.Body; }
            public string SubredditWrap { get => _base.Subreddit; }
        }
    }
}