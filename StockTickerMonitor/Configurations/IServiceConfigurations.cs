namespace StockTickerMonitor.Configurations
{
    public interface IServiceConfigurations
    {
        string QueueHost { get; }
        string QueueExchange { get; }
        string Queue { get; }
        string QueueUserName{ get; }
        string QueuePassword { get; }
        string RedditAppId { get; }
        string RedditOAuthKey { get; }
        string TwitterApiKey { get; }
        string TwitterApiSecret { get; }
        string TwitterAccessToken { get; }
        string TwitterAccessSecret { get; }
        string TwitterBearerToken { get; }
        string ApiUrl { get; }
        int QueuePort { get; }
        bool SslEnabled { get; }
    }
}