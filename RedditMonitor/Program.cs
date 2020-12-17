using System;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.ObjectPool;
using RabbitMQ.Client;
using RedditMonitor.Logic;
using RedditMonitor.Logic.RabbitMQ;

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
            services.AddSingleton<IRabbitManager, RabbitManager>();
            _serviceProvider = services.BuildServiceProvider(true);
            // var builder = new ContainerBuilder();
            // builder.RegisterType<ObjectPoolProvider>().As<DefaultObjectPoolProvider>().SingleInstance();
            // builder.RegisterType<IPooledObjectPolicy<IModel>>().As<RabbitMqConnection>().SingleInstance();
            // builder.RegisterType<RedditMonitoring>().As<IRedditMonitoring>();
            // builder.Populate(collection);
            // var appContainer = builder.Build();

            // _serviceProvider = new AutofacServiceProvider(appContainer);
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
