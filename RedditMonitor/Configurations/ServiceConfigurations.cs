using System.Configuration;
using Common.RabbitMQ;
using Microsoft.Extensions.Configuration;

namespace RedditMonitor.Configurations
{
    public class ServiceConfigurations : IServiceConfigurations, IRabbitConfigurations
    {
        public ServiceConfigurations(IConfiguration configs)
        {
            QueueHost = configs["rabbitmq:host"];
            QueueExchange = configs["rabbitmq:exchange"];
            Queue = configs["rabbitmq:queue"];
            RedditAppId = configs["redditApi:appId"];
            RedditOAuthKey = configs["redditApi:oauthKey"];
            TwitterApiKey = configs["twitterApi:apiKey"];
            TwitterApiSecret = configs["twitterApi:apiSecret"];
            TwitterAccessToken = configs["twitterApi:accessToken"];
            TwitterAccessSecret = configs["twitterApi:accessSecret"];
            TwitterBearerToken = configs["twitter:bearerToken"];
            ApiUrl = configs["api:url"];
        }

        public string QueueHost { get; }
        public string QueueExchange { get; }
        public string Queue { get; }
        public string RedditAppId { get; }
        public string RedditOAuthKey { get; }
        public string TwitterApiKey { get; }
        public string TwitterApiSecret { get; }
        public string TwitterAccessToken { get; }
        public string TwitterAccessSecret { get; }
        public string TwitterBearerToken { get; }
        public string ApiUrl { get; }
    }
}