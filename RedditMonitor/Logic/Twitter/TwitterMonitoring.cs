using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net.Http;
using System.Threading.Tasks;
using Common.Models;
using Common.RabbitMQ;
using Newtonsoft.Json;
using Tweetinvi;
using Tweetinvi.Events;
using Tweetinvi.Models;
using Tweetinvi.Streaming;

namespace RedditMonitor.Logic.Twitter
{
    public class TwitterMonitoring : ITwitterMonitoring
    {
        private readonly IRabbitPublisher _rabbitPublisher;
        private readonly ITwitterClient _twitterClient;
        private IFilteredStream _twitterStream;
        private IEnumerable<StockTickerDb> _tickers;
        private DateTime _lastUpdateTime;
        private readonly String _routingKey = "reddit-comments";

        public TwitterMonitoring(
            ITwitterClient twitterClient,
            IRabbitPublisher rabbitPublisher)
        {
            _twitterClient = twitterClient;
            _rabbitPublisher = rabbitPublisher;
            _tickers = LoadTickersAsync().Result;
        }

        public async Task MonitorTweetsAsync()
        {
            _twitterStream = _twitterClient.Streams.CreateFilteredStream();
            SetStreamTrackers();
            _twitterStream.MatchingTweetReceived += C_ProcessTweet;
            await _twitterStream.StartMatchingAnyConditionAsync();
        }

        private void SetStreamTrackers()
        {
            foreach(var t in _tickers)
                _twitterStream.AddTrack($"${t.NasdaqSymbol}");
        }

        private async Task<IEnumerable<StockTickerDb>> LoadTickersAsync()
        {
            using (var httpClientHandler = new HttpClientHandler())
            {
                httpClientHandler.ServerCertificateCustomValidationCallback = (message, cert, chain, sslPolicyErrors) => true;
                using(var httpClient = new HttpClient(httpClientHandler))
                {
                    var stockTickers = await httpClient.GetAsync(BuildUrl());
                    _tickers = JsonConvert.DeserializeObject<List<StockTickerDb>>(
                            await stockTickers.Content.ReadAsStringAsync());
                }
            }
            _lastUpdateTime = DateTime.Now;
            return _tickers;
        }

        private string BuildUrl()
            => $"http://localhost:5000/stockticker/gettoptickers?limit={400}";

        private void C_ProcessTweet(object sender, MatchedTweetReceivedEventArgs args)
        {
            var message = BuildQueueMessage(args.Tweet);
            _rabbitPublisher.Publish<QueueMessage>(message, _routingKey);
            CheckTimeToUpdateTickers();
        }

        private async void CheckTimeToUpdateTickers()
        {
            if (DateTime.Now.Hour - _lastUpdateTime.Hour > 1)
            {
                _twitterStream.Stop();
                await LoadTickersAsync();
                _twitterStream = _twitterClient.Streams.CreateFilteredStream();
                SetStreamTrackers();
                await _twitterStream.StartMatchingAnyConditionAsync();
            }
        }

        private QueueMessage BuildQueueMessage(ITweet tweet)
        {
            var tweetMessage = tweet.FullText;
            var quotedTweetMessage = tweet.QuotedTweet?.FullText ?? "";
            var retweetMessage = tweet.RetweetedTweet?.FullText ?? "";
            var fullMessage = $"{tweetMessage} {quotedTweetMessage} {retweetMessage}";
            return new QueueMessage
            {
                TraceId = Guid.NewGuid(),
                MessageContent = new FoundMessage {
                    Source = MessageSource.Twitter,
                    SubReddit = null,
                    RedditId = tweet.Id.ToString(),
                    TimePosted = tweet.CreatedAt.UtcDateTime,
                    Message = fullMessage
                }
            };
        }
    }
}