using RabbitMQ.Client.Events;

namespace Common.RabbitMQ
{
    public interface IRabbitConsumer
    {
        AsyncEventingBasicConsumer GetAsyncConsumer();
        void RegisterConsumer(AsyncEventingBasicConsumer consumer);
        void BasicAck(ulong deliveryTag, bool multimessage);
        void BasicReject(ulong deliveryTag, bool reque);
    }
}