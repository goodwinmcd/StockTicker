using Microsoft.Extensions.Configuration;

namespace RedditMonitor.Configurations
{
    public class ServiceConfigurations : IServiceConfigurations
    {
        public ServiceConfigurations(IConfiguration configs)
        {
            dbConnectionString = configs.GetConnectionString("stockTickerConnectionString");
        }

        public string dbConnectionString { get; }
    }
}