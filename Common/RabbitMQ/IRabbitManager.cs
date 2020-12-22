using RabbitMQ.Client.Events;

namespace Common.RabbitMQ
{
    public interface IRabbitManager
    {
        void Publish<T>(T message, string routeKey) where T : class;
        EventingBasicConsumer GetAsyncConsumer();
        void RegisterConsumer(EventingBasicConsumer consumer);
        void BasicAck(ulong deliveryTag, bool multimessage);
    }
}