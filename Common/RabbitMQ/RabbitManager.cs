using System;
using System.Collections.Generic;
using System.Threading;
using RabbitMQ.Client;

namespace Common.RabbitMQ
{
    public abstract class RabbitManager
    {
        protected static IConnection _connection;
        protected static IModel _channel;
        protected static String _exchangeName;
        protected static String _queueName;
        protected static String _rabbitHost;
        protected static string _username;
        protected static string _password;
        protected static bool _sslEnabled;
        protected static int _port;

        public RabbitManager(IRabbitConfigurations configs)
        {
            _exchangeName = configs.QueueExchange;
            _queueName = configs.Queue;
            _rabbitHost = configs.QueueHost;
            _port = configs.QueuePort;
            _sslEnabled = configs.SslEnabled;
            _username = configs.QueueUserName;
            _password = configs.QueuePassword;
            Connect();
        }

        private static void Connect()
        {
            var factory = new ConnectionFactory()
            {
                HostName = _rabbitHost,
                UserName = _username,
                Password = _password,
                Port = _port,
                RequestedHeartbeat = new TimeSpan(30),
                DispatchConsumersAsync = true,
            };
            factory.Ssl.Enabled = _sslEnabled;
            // Don't give a damn about certs
            factory.Ssl.AcceptablePolicyErrors = System.Net.Security.SslPolicyErrors.RemoteCertificateChainErrors
                                            | System.Net.Security.SslPolicyErrors.RemoteCertificateNameMismatch
                                            | System.Net.Security.SslPolicyErrors.RemoteCertificateNotAvailable;
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