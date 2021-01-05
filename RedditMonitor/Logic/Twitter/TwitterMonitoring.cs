using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Threading;
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
            try
            {
                await _twitterStream.StartMatchingAnyConditionAsync();
            }
            catch
            {
                Thread.Sleep(5000);
                await MonitorTweetsAsync();
            }
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
                    var stockTickers = await httpClient.GetAsync(BuildTopTickersUrl());
                    _tickers = JsonConvert.DeserializeObject<List<StockTickerDb>>(
                            await stockTickers.Content.ReadAsStringAsync());
                    if (_tickers.Count() == 0)
                    {
                        var allTickers = await httpClient.GetAsync(BuildAllTickersUrl());
                        var tempAllTickers = JsonConvert.DeserializeObject<List<StockTickerDb>>(
                            await allTickers.Content.ReadAsStringAsync());
                        _tickers = tempAllTickers.Take(400);
                    }
                }
            }
            _lastUpdateTime = DateTime.Now;
            return _tickers;
        }

        private string BuildTopTickersUrl()
            => $"http://localhost:5000/stockticker/gettoptickers?limit={400}&getVolume={false}&source={MessageSource.Reddit.ToString()}";

        private string BuildAllTickersUrl()
            => $"http://localhost:5000/stockticker";

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
                try
                {
                    await _twitterStream.StartMatchingAnyConditionAsync();
                }
                catch
                {
                    await MonitorTweetsAsync();
                }
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
                    Source = MessageSource.Twitter.ToString(),
                    SubReddit = null,
                    RedditId = tweet.Id.ToString(),
                    TimePosted = tweet.CreatedAt.UtcDateTime,
                    Message = fullMessage
                }
            };
        }
    }
}