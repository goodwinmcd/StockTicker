using System.Configuration;
using Common.RabbitMQ;
using Microsoft.Extensions.Configuration;

namespace RedditMonitorWorker.ServiceConfiguration
{
    public class ServiceConfigurations : IServiceConfigurations, IRabbitConfigurations
    {
        public ServiceConfigurations(IConfiguration configs)
        {
            QueueHost = configs["rabbitmq:host"];
            QueueExchange = configs["rabbitmq:exchange"];
            Queue = configs["rabbitmq:queue"];
            ApiUrl = configs["api:url"];
        }

        public string QueueHost { get; }
        public string QueueExchange { get; }
        public string Queue { get; }
        public string ApiUrl { get; }
    }
}