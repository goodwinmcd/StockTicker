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
        private IEnumerable<String> _stockTickers = new List<String>();

        public RedditConsumer(IRabbitManager rabbitManager)
        {
            _rabbitManager = rabbitManager;
        }

        public void Consume()
        {
            var consumer = _rabbitManager.GetAsyncConsumer();

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
        }

        private string StripPunctuation(string s)
        {
            var sb = new StringBuilder();
            foreach (char c in s)
            {
                if (!char.IsPunctuation(c) || c != '$')
                    sb.Append(Char.ToLower(c));
            }
            return sb.ToString();
        }
    }
}