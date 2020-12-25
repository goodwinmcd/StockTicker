using System.Text;
using Newtonsoft.Json;
using RabbitMQ.Client;

namespace Common.RabbitMQ
{
    public class RabbitPublisher : RabbitManager, IRabbitPublisher
    {
        public void Publish<T>(T message, string routeKey) where T : class
        {
            if (message == null)
                return;

            var body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message));
                _channel.BasicPublish(
                    exchange: _exchangeName,
                    routingKey: "reddit-comments",
                    basicProperties: null,
                    body: body);
        }
    }
}