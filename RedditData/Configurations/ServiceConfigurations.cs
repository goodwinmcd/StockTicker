using System.Configuration;
using Common.RabbitMQ;

namespace RedditData.Configurations
{
    public class ServiceConfigurations : IServiceConfigurations
    {
        public string ApiUrl { get; } = ConfigurationManager.AppSettings["api_url"];
    }
}