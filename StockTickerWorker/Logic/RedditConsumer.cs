using System.Text;
using Newtonsoft.Json;
using RabbitMQ.Client.Events;
using System.Linq;
using System;
using System.Net.Http;
using System.Net;
using System.Threading.Tasks;
using Common.RabbitMQ;
using System.Text.RegularExpressions;
using StockTickerWorker.ServiceConfiguration;
using StockTickerWorker.Models;

namespace StockTickerWorker.Logic
{
    public class RedditConsumer : IRedditConsumer
    {
        public readonly  IRabbitPublisher _rabbitPublisher;
        public readonly  IRabbitConsumer _rabbitConsumer;
        private readonly IStockTickerManager _stockTickerManager;
        private readonly IServiceConfigurations _serviceConfigurations;
        private readonly ISentimentAnalysis _sentimentAnalysis;
        private readonly String _routingKey = "messagesToProcess";

        public RedditConsumer(
            IRabbitPublisher rabbitPublisher,
            IRabbitConsumer rabbitManager,
            IStockTickerManager stockTickerManager,
            IServiceConfigurations serviceConfigurations,
            ISentimentAnalysis sentimentAnalysis)
        {
            _rabbitPublisher = rabbitPublisher;
            _rabbitConsumer = rabbitManager;
            _stockTickerManager = stockTickerManager;
            _serviceConfigurations = serviceConfigurations;
            _sentimentAnalysis = sentimentAnalysis;
        }

        public void Consume()
        {
            var consumer = _rabbitConsumer.GetAsyncConsumer();
            consumer.Received += C_ConsumeMessage;
            _rabbitConsumer.RegisterConsumer(consumer);
        }

        private async Task C_ConsumeMessage(object ch, BasicDeliverEventArgs ea)
        {
            var content = Encoding.UTF8.GetString(ea.Body.ToArray());
            var body = JsonConvert.DeserializeObject<QueueMessage>(content);
            var messageWords = StripNewLines(StripPunctuation(body.MessageContent.Message)).Split(' ');
            var foundStockTickers = _stockTickerManager.FindMatchingTickers(messageWords);
            if (foundStockTickers.Any())
            {
                body.MessageContent.Sentiment =
                    await _sentimentAnalysis.GetSentimentAsIntAsync(body.MessageContent.Message);
                body.MessageContent.Tickers = foundStockTickers;
                try
                {
                    if(await CallApi(body.MessageContent))
                        _rabbitConsumer.BasicAck(ea.DeliveryTag, false);
                    else
                        CheckRetryAndReque(body, ch, ea);
                }
                catch
                {
                    CheckRetryAndReque(body, ch, ea);
                }
            }
            else{
                _rabbitConsumer.BasicAck(ea.DeliveryTag, false);
            }
        }

        private void CheckRetryAndReque(QueueMessage message, object ch, BasicDeliverEventArgs ea)
        {
            if (message.RetryCount < 3)
            {
                message.RetryCount++;
                _rabbitConsumer.BasicAck(ea.DeliveryTag, false);
                _rabbitPublisher.Publish<QueueMessage>(message, _routingKey);
            }
            else
            {
                _rabbitConsumer.BasicReject(ea.DeliveryTag, false);
            }
        }

        private async Task<bool> CallApi(FoundMessage message)
        {
                var content = ConvertToJson(message);

                using (var httpClientHandler = new HttpClientHandler())
                using (var httpClient = new HttpClient(httpClientHandler))
                {
                    httpClientHandler.ServerCertificateCustomValidationCallback = (message, cert, chain, sslPolicyErrors) => {
                        return true;
                    };
                    var result = await httpClient.PostAsync($"{_serviceConfigurations.ApiUrl}/Message", content);
                    return result.StatusCode == HttpStatusCode.Created;
                }
        }

        private StringContent ConvertToJson(FoundMessage message) =>
            new StringContent(JsonConvert.SerializeObject(message), Encoding.UTF8, "application/json");

        private string StripNewLines(string s)
            => s = Regex.Replace(s, @"(?:\r\n|[\r\n])", " ");

        private string StripPunctuation(string s)
        {
            var sb = new StringBuilder();
            foreach (char c in s)
            {
                if (char.IsLetter(c) || char.IsWhiteSpace(c))
                    sb.Append(c);
            }
            return sb.ToString();
        }
    }
}