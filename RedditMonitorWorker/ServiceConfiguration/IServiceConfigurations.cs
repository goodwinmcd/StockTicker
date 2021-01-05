namespace RedditMonitorWorker.ServiceConfiguration
{
    public interface IServiceConfigurations
    {
        public string QueueHost { get; }
        public string QueueExchange { get; }
        public string Queue { get; }
        public string RedditAppId { get; }
        public string RedditOauthKey { get; }
        public string ApiUrl { get; }
    }
}