namespace Common.RabbitMQ
{
    public interface IRabbitConfigurations
    {
        string Queue { get; }
        string QueueHost { get; }
        string QueueExchange { get; }

    }
}