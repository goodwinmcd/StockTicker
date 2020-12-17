using System.Configuration;
using RabbitMQ.Client;
using Reddit;
using Reddit.Controllers.EventArgs;
using RedditMonitor.Logic.RabbitMQ;

namespace RedditMonitor.Logic
{
    public class RedditMonitoring : IRedditMonitoring
    {
         private enum subreddits
        {
            wallstreetbets,
            investing,
            securityanalysis,
            robinhood,
            robinhoodpennystocks,
            stocks,
            economics,
            options,
            pennystocksdd,
            pennystocks,
            finance,
            forex,
            stock_picks,
            stockmarket,
            investmentclub,
            algotrading,
        }

        private readonly IRabbitManager _rabbitManager;

        public RedditMonitoring(IRabbitManager rabbitManager)
        {
            _rabbitManager = rabbitManager;
        }

        public void MonitorPosts()
        {
            var reddit = new RedditClient(
                ConfigurationManager.AppSettings["reddit_app_id"],
                ConfigurationManager.AppSettings["reddit_oauth_key"]);

            var wallStreetBets = reddit.Subreddit("wallstreetbets");
            wallStreetBets.Posts.GetNew();
            wallStreetBets.Comments.GetNew();  // This call prevents any existing "new"-sorted comments from triggering the update event.  --Kris
			wallStreetBets.Comments.MonitorNew();
			wallStreetBets.Comments.NewUpdated += C_AddNewPostToQueue;
        }

        private static void C_AddNewPostToQueue(
            object sender,
            CommentsUpdateEventArgs eventArgs)
        {
            var dept = ConfigurationManager.AppSettings["rabbitmqHost"];

        }
    }
}