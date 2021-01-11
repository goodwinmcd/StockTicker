using System;
using Common.RabbitMQ;
using Microsoft.Extensions.Configuration;

namespace RedditMonitor.Configurations
{
    public class ServiceConfigurations : IServiceConfigurations, IRabbitConfigurations
    {
        public ServiceConfigurations(IConfiguration configs)
        {
            QueueHost = configs["rabbitmq:hostname"];
            QueueExchange = configs["rabbitmq:exchange"];
            Queue = configs["rabbitmq:queue"];
            QueueUserName = configs["rabbitmq:username"];
            QueuePassword = configs["rabbitmq:password"];
            QueuePort = Int32.Parse(configs["rabbitmq:port"]);
            SslEnabled = Boolean.Parse(configs["rabbitmq:sslEnabled"]);
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
        public int QueuePort { get; }
        public bool SslEnabled { get; }
        public string QueueUserName { get; }
        public string QueuePassword { get; }
    }
}