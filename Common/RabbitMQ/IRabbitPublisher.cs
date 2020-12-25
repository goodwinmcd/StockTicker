namespace Common.RabbitMQ
{
    public interface IRabbitPublisher
    {
        void Publish<T>(T message, string routeKey) where T : class;
    }
}