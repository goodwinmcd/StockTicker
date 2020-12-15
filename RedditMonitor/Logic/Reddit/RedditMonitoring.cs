using System.Configuration;
using RabbitMQ.Client;
using Reddit;
using Reddit.Controllers.EventArgs;

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

        private static void C_AddNewPostToQueue(object sender, CommentsUpdateEventArgs eventArgs)
        {
            var dept = ConfigurationManager.AppSettings["rabbitmqHost"];
            var factory = new ConnectionFactory()
            {
                HostName = ConfigurationManager.AppSettings["rabbitmqHost"]
            };
            // foreach (Comment comment in eventArgs.Added)
            // {
            //     using(var connection = factory.CreateConnection())
            //     using(var channel = connection.CreateModel())
            //     {
            //         channel.QueueDeclare(queue: "reddit-comments",
            //                             durable: false,
            //                             exclusive: false,
            //                             autoDelete: false,
            //                             arguments: null);

            //         var body = Encoding.UTF8.GetBytes(message);

            //         channel.BasicPublish(exchange: "",
            //                             routingKey: "reddit-comments",
            //                             basicProperties: null,
            //                             body: body);
            //         Console.WriteLine(" [x] Sent {0}", message);
                // }
            // }
        }
    }
}