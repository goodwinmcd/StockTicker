using System.Configuration;
using Common.RabbitMQ;

namespace RedditMonitor.Configurations
{
    public class ServiceConfigurations : IServiceConfigurations, IRabbitConfigurations
    {
        public string QueueHost { get; } = ConfigurationManager.AppSettings["rabbitmqHost"];
        public string QueueExchange { get; } = ConfigurationManager.AppSettings["rabbitExchange"];
        public string Queue { get; } = ConfigurationManager.AppSettings["rabbitQueue"];
        public string RedditAppId { get; } = ConfigurationManager.AppSettings["reddit_app_id"];
        public string RedditOAuthKey { get; } = ConfigurationManager.AppSettings["reddit_oauth_key"];
        public string TwitterApiKey { get; } = ConfigurationManager.AppSettings["twitter_api_key"];
        public string TwitterApiSecret { get; } = ConfigurationManager.AppSettings["twitter_api_secret"];
        public string TwitterAccessToken { get; } = ConfigurationManager.AppSettings["twitter_access_token"];
        public string TwitterAccessSecret { get; } = ConfigurationManager.AppSettings["twitter_access_secret"];
        public string TwitterBearerToken { get; } = ConfigurationManager.AppSettings["twitter_bearer_token"];
        public string ApiUrl { get; } = ConfigurationManager.AppSettings["api_url"];
    }
}