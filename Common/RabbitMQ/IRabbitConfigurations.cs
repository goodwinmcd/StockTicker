namespace Common.RabbitMQ
{
    public interface IRabbitConfigurations
    {
        string Queue { get; }
        string QueueHost { get; }
        string QueueExchange { get; }
        int QueuePort { get; }
        bool SslEnabled { get; }
        string QueueUserName { get; }
        string QueuePassword { get; }
    }
}