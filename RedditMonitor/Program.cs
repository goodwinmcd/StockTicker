using System;
using System.Text;
using RabbitMQ.Client;
using Reddit;
using Reddit.Controllers;
using Reddit.Controllers.EventArgs;

namespace RedditMonitor
{
    class Program
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

        static void Main(string[] args)
        {
            var reddit = new RedditClient("mGqzdWydsbEojw", "31745057-ZFtdV83BXkOqklWcsaKyMwNbY2cV1Q");
            var wallStreetBets = reddit.Subreddit("wallstreetbets");
            wallStreetBets.Posts.GetNew();
            wallStreetBets.Comments.GetNew();  // This call prevents any existing "new"-sorted comments from triggering the update event.  --Kris
			wallStreetBets.Comments.MonitorNew();
			wallStreetBets.Comments.NewUpdated += C_AddNewPostToQueue;
        }

        private static void C_AddNewPostToQueue(object sender, CommentsUpdateEventArgs eventArgs)
        {
            Console.WriteLine("New Message");
            // var factory = new ConnectionFactory() { HostName = "localhost" };
            // foreach (Comment comment in eventArgs.Added)
            // {
            //     using(var connection = factory.CreateConnection())
            //     using(var channel = connection.CreateModel())
            //     {
            //         channel.QueueDeclare(queue: "hello",
            //                             durable: false,
            //                             exclusive: false,
            //                             autoDelete: false,
            //                             arguments: null);

            //         string message = "Hello World!";
            //         var body = Encoding.UTF8.GetBytes(message);

            //         channel.BasicPublish(exchange: "",
            //                             routingKey: "hello",
            //                             basicProperties: null,
            //                             body: body);
            //         Console.WriteLine(" [x] Sent {0}", message);
                // }
            // }
        }
    }
}
