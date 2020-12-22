using System.Text;
using Common.Models;
using Common.RabbitMQ;
using Newtonsoft.Json;
using RabbitMQ.Client.Events;
using System.Linq;
using System;
using System.Collections.Generic;
namespace RedditMonitorWorker.Logic
{
    public class RedditConsumer : IRedditConsumer
    {
        public readonly IRabbitManager _rabbitManager;
        private IEnumerable<string> _stockTickers;

        public RedditConsumer(
            IRabbitManager rabbitManager,
            IStockTickerManager StockTickerManager)
        {
            _rabbitManager = rabbitManager;
            _stockTickers = StockTickerManager.AllStockTickers;
        }

        public void Consume()
        {
            var consumer = _rabbitManager.GetAsyncConsumer();
            consumer.Received += C_ConsumeMessage;
            _rabbitManager.RegisterConsumer(consumer);
        }

        private void C_ConsumeMessage(object ch, BasicDeliverEventArgs ea)
        {
            var content = Encoding.UTF8.GetString(ea.Body.ToArray());
            var body = JsonConvert.DeserializeObject<RedditQueueMessage>(content);
            var messageWords = StripPunctuation(body.Message).Split(' ');
            var foundStockTickers = _stockTickers.Intersect(messageWords);
            if (foundStockTickers.Any())
            {
                // call api to store value
            }
            _rabbitManager.BasicAck(ea.DeliveryTag, false);
        }

        private string StripPunctuation(string s)
        {
            var sb = new StringBuilder();
            foreach (char c in s)
            {
                if (char.IsLetter(c) || char.IsWhiteSpace(c))
                    sb.Append(Char.ToLower(c));
            }
            return sb.ToString();
        }
    }
}