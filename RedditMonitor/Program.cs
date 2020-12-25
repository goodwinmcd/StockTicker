using System;
using Common.RabbitMQ;
using Microsoft.Extensions.DependencyInjection;
using RedditMonitor.Logic;

namespace RedditMonitor
{
    class Program
    {
        private static IServiceProvider _serviceProvider;

        static void Main(string[] args)
        {
            RegisterServices();
            var redditMonitoring = _serviceProvider.GetService<IRedditMonitoring>();
            redditMonitoring.MonitorPosts();
            DisposeServices();
        }

        private static void RegisterServices()
        {
            var services = new ServiceCollection();
            services.AddSingleton<IRedditMonitoring, RedditMonitoring>();
            services.AddSingleton<IRabbitPublisher, RabbitPublisher>();
            _serviceProvider = services.BuildServiceProvider(true);
        }

        private static void DisposeServices()
        {
            if(_serviceProvider == null)
                return;
            if (_serviceProvider is IDisposable)
                ((IDisposable)_serviceProvider).Dispose();
        }
    }
}
