using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Threading;
using RabbitMQ.Client;

namespace Common.RabbitMQ
{
    public abstract class RabbitManager
    {
        protected static IConnection _connection;
        protected static IModel _channel;
        protected static readonly String _exchangeName = ConfigurationManager.AppSettings["rabbitExchange"];
        protected static readonly String _queueName = ConfigurationManager.AppSettings["rabbitQueue"];
        protected static readonly String _rabbitHost = ConfigurationManager.AppSettings["rabbitmqHost"];

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
                DispatchConsumersAsync = true,
            };
            _connection = factory.CreateConnection();
            _connection.ConnectionShutdown += C_ConnectionShutdown;
            _channel = _connection.CreateModel();
            _channel.ExchangeDeclare(_exchangeName, ExchangeType.Direct);
            IDictionary<string, string> args = new Dictionary<string, string>()
            {
                {"x-dead-letter-exchange", ""},
            };
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
            catch
            {
                // Close() may throw an IOException if connection
                // dies - but that's ok (handled by reconnect)
            }
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
                catch
                {
                    Console.WriteLine("Reconnect failed!");
                    Thread.Sleep(3000);
                }
            }
        }


    }
}