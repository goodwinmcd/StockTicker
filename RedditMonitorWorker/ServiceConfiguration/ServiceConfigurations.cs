using System;
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
            QueuePort = Int32.Parse(configs["rabbitmq:port"]);
            Queue = configs["rabbitmq:queue"];
            SslEnabled = Boolean.Parse(configs["rabbitmq:sslEnabled"]);
            QueueUserName = configs["rabbitmq:username"];
            QueuePassword = configs["rabbitmq:password"];
            ApiUrl = configs["api:url"];
        }

        public string QueueHost { get; }
        public string QueueExchange { get; }
        public int QueuePort { get; }
        public string Queue { get; }
        public string ApiUrl { get; }
        public bool SslEnabled { get; }
        public string QueueUserName { get; }
        public string QueuePassword { get; }
    }
}