using System.Configuration;
using Common.RabbitMQ;

namespace RedditMonitorWorker.ServiceConfiguration
{
    public class ServiceConfigurations : IServiceConfigurations, IRabbitConfigurations
    {
        public string QueueHost { get; } = ConfigurationManager.AppSettings["rabbitmqHost"];
        public string QueueExchange { get; } = ConfigurationManager.AppSettings["rabbitExchange"];
        public string Queue { get; } = ConfigurationManager.AppSettings["rabbitQueue"];
        public string RedditAppId { get; } = ConfigurationManager.AppSettings["reddit_app_id"];
        public string RedditOauthKey { get; } = ConfigurationManager.AppSettings["reddit_oauth_key"];
        public string ApiUrl { get; } = ConfigurationManager.AppSettings["api_url"];
    }
}