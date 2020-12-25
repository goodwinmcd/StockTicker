using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Common.RabbitMQ
{
    public class RabbitConsumer :  RabbitManager, IRabbitConsumer
    {
        public AsyncEventingBasicConsumer GetAsyncConsumer()
            => new AsyncEventingBasicConsumer(_channel);

        public void RegisterConsumer(AsyncEventingBasicConsumer consumer)
        {
            _channel.BasicConsume(queue: _queueName, consumer: consumer);
        }

        public void BasicAck(ulong deliveryTag, bool multimessage)
        {
            _channel.BasicAck(deliveryTag, multimessage);
        }

        public void BasicReject(ulong deliveryTag, bool requeue)
            => _channel.BasicReject(deliveryTag, requeue);
    }
}