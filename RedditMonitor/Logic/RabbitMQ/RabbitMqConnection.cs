using Microsoft.Extensions.ObjectPool;
using RabbitMQ.Client;
using System.Configuration;

namespace RedditMonitor.Logic
{
    public class RabbitMqConnection : IPooledObjectPolicy<IModel>
    {
        IConnection _connection;

        public RabbitMqConnection()
        {
            _connection = GetConnection();
        }

        public IConnection GetConnection()
        {
            var factory = new ConnectionFactory()
            {
                HostName = ConfigurationManager.AppSettings["rabbitmqHost"]
            };
            return factory.CreateConnection();
        }

        public IModel Create()
        {
            return _connection.CreateModel();
        }

        public bool Return(IModel obj)
        {
            if (obj.IsOpen)
            {
                return true;
            }
            else
            {
                obj?.Dispose();
                return false;
            }
        }
    }
}