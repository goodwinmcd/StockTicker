using System;
using System.Configuration;
using System.IO;
using System.Text;
using System.Threading;
using Microsoft.Extensions.ObjectPool;
using Newtonsoft.Json;
using RabbitMQ.Client;

namespace RedditMonitor.Logic.RabbitMQ
{
    public class RabbitManager : IRabbitManager
    {
        private static IConnection _connection;
        private static IModel _channel;
        private static readonly String _exchangeName = ConfigurationManager.AppSettings["rabbitExchange"];
        private static readonly String _queueName = ConfigurationManager.AppSettings["rabbitQueue"];
        private static readonly String _rabbitHost = ConfigurationManager.AppSettings["rabbitmqHost"];

        public RabbitManager()
        {
            Connect();
        }

        private static void Connect()
        {
            var factory = new ConnectionFactory()
            {
                HostName = _rabbitHost,
                RequestedHeartbeat = new TimeSpan(30),
            };
            _connection = factory.CreateConnection();
            _connection.ConnectionShutdown += C_ConnectionShutdown;
            _channel = _connection.CreateModel();
            _channel.ExchangeDeclare(_exchangeName, ExchangeType.Direct);
            _channel.QueueDeclare(_queueName, false, false, false, null);
            _channel.QueueBind(_queueName, _exchangeName, _queueName, null);
        }

        private static void Cleanup()
        {
            try
            {
                if (_channel != null && _channel.IsOpen)
                {
                    _channel.Close();
                    _channel = null;
                }

                if (_connection != null && _connection.IsOpen)
                {
                    _connection.Close();
                    _connection = null;
                }
            }
            catch(IOException ex)
            {
                // Close() may throw an IOException if connection
                // dies - but that's ok (handled by reconnect)
            }
        }

        public void Publish<T>(T message, string routeKey) where T : class
        {
            if (message == null)
                return;

            var body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message));
            _channel.BasicPublish(exchange: _exchangeName,
                routingKey: "reddit-comments",
                basicProperties: null,
                body: body);
        }

        private static void C_ConnectionShutdown(
            object sender,
            ShutdownEventArgs e)
        {
            Console.WriteLine("Connection broke!");
            Cleanup();
            while (true)
            {
                try
                {
                    Connect();
                    Console.WriteLine("Reconnected!");
                    break;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Reconnect failed!");
                    Thread.Sleep(3000);
                }
            }
        }
    }
}