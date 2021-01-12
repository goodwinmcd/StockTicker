using System.Collections.Generic;
using System.Threading.Tasks;
using Amazon.Comprehend;
using Amazon.Comprehend.Model;

namespace StockTickerWorker.Logic
{
    public class SentimentAnalysis : ISentimentAnalysis
    {
        private AmazonComprehendClient _awsComprehendClient;
        private Dictionary<SentimentType, int> _sentimentToIntMapper = new Dictionary<SentimentType, int>()
        {
            { SentimentType.NEGATIVE, -1 },
            { SentimentType.NEUTRAL, 0},
            { SentimentType.MIXED, 0},
            { SentimentType.POSITIVE, 1},
        };

        public SentimentAnalysis()
        {
            _awsComprehendClient = new AmazonComprehendClient();
        }

        public async Task<int> GetSentimentAsIntAsync(string messageText)
        {
            var request = new DetectSentimentRequest() { Text = messageText, LanguageCode = LanguageCode.En };
            var sentiment = await _awsComprehendClient.DetectSentimentAsync(request);
            return ParseSentimentToInt(sentiment);
        }

        private int ParseSentimentToInt(DetectSentimentResponse awsSentiment)
            => _sentimentToIntMapper.GetValueOrDefault(awsSentiment.Sentiment, 0);
    }
}