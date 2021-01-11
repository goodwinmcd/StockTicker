using Microsoft.Extensions.Configuration;

namespace StockTickerUi.Configurations
{
    public class ServiceConfigurations : IServiceConfigurations
    {
        public ServiceConfigurations(IConfiguration configurations)
        {
            ApiUrl = configurations["api_url"];
        }

        public string ApiUrl { get; }
    }
}