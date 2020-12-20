using RabbitMQ.Client.Events;

namespace Common.RabbitMQ
{
    public interface IRabbitManager
    {
        void Publish<T>(T message, string routeKey) where T : class;
        AsyncEventingBasicConsumer GetAsyncConsumer();
    }
}