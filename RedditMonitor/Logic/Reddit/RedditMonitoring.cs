using System;
using System.Configuration;
using Common.Models;
using Common.RabbitMQ;
using Reddit;
using Reddit.Controllers;
using Reddit.Controllers.EventArgs;

namespace RedditMonitor.Logic
{
    public class RedditMonitoring : IRedditMonitoring
    {
        private readonly IRabbitManager _rabbitManager;
        private readonly String redditAppId = ConfigurationManager.AppSettings["reddit_app_id"];
        private readonly String redditOauthKey = ConfigurationManager.AppSettings["reddit_oauth_key"];
        private readonly String routingKey = "reddit-comments";

        public RedditMonitoring(IRabbitManager rabbitManager)
        {
            _rabbitManager = rabbitManager;
        }

        public void MonitorPosts()
        {
            var reddit = new RedditClient(redditAppId, redditOauthKey);

            var wallStreetBets = reddit.Subreddit("wallstreetbets");
            wallStreetBets.Posts.GetNew();
            wallStreetBets.Comments.GetNew();  // This call prevents any existing "new"-sorted comments from triggering the update event.  --Kris
			wallStreetBets.Comments.MonitorNew();
			wallStreetBets.Comments.NewUpdated += C_AddNewPostToQueue;
        }

        private void C_AddNewPostToQueue(
            object sender,
            CommentsUpdateEventArgs eventArgs)
        {
            foreach (var comment in eventArgs.Added)
            {
                var payload = buildRedditQueueMessage(sender, comment);
                _rabbitManager.Publish<RedditQueueMessage>(payload, routingKey);

            }
            var dept = ConfigurationManager.AppSettings["rabbitmqHost"];
        }

        private RedditQueueMessage buildRedditQueueMessage(object sender, Comment comment)
        {
            if (!Enum.TryParse(comment.Subreddit, out SubReddit subReddit))
                Console.WriteLine($"Unable to parse subreddit: {comment.Subreddit}");
            return new RedditQueueMessage
                {
                    Source = RedditMessageType.Comment,
                    SubReddit = subReddit,
                    RedditId = comment.Id,
                    TimePosted = comment.Created,
                    Message = comment.Body,
                };
        }
    }
}