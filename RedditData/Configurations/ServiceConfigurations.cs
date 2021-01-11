using Microsoft.Extensions.Configuration;

namespace RedditData.Configurations
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