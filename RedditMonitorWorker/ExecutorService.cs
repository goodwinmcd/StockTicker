using System;
using Autofac;
using Common.RabbitMQ;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RedditMonitorWorker.Logic;
using RedditMonitorWorker.ServiceConfiguration;
using Topshelf;

namespace RedditMonitorWorker
{
    public class ExecutorService
    {

        private static IRedditConsumer _worker;
        private static IContainer _container;
        private static IServiceProvider _serviceProvider;

        private static void BootstrapService()
        {
            Configure();
            _worker = _serviceProvider.GetService<IRedditConsumer>();
        }

        public bool Start(HostControl hostControl)
        {
            BootstrapService();
            try
            {
                _worker.Consume();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool Stop(HostControl hostControl)
        {
            DisposeServices();
            return true;
        }

        private static void Configure()
        {
            //Registration of Dependencies
            RegisterDependencies();
        }

        private static void RegisterDependencies()
        {
            var services = new ServiceCollection();
            var appConfigurations = RegisterConfigurations();
            var configurations = new ServiceConfigurations(appConfigurations);
            services.AddSingleton<IRabbitConfigurations>(configurations);
            services.AddSingleton<IServiceConfigurations>(configurations);
            services.AddSingleton<IRabbitConsumer, RabbitConsumer>();
            services.AddSingleton<IStockTickerManager, StockTickerManager>();
            services.AddSingleton<IRedditConsumer, RedditConsumer>();
            services.AddSingleton<IRabbitPublisher, RabbitPublisher>();
            _serviceProvider = services.BuildServiceProvider(true);
        }

        private static IConfiguration RegisterConfigurations()
            => new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .AddEnvironmentVariables()
                .AddSystemsManager(awsConfigs => {
                    awsConfigs.AwsOptions = new Amazon.Extensions.NETCore.Setup.AWSOptions
                    {
                        Profile = "socialmediadata",
                        Region = Amazon.RegionEndpoint.USWest2
                    };
                    awsConfigs.Path = "/SocialMedia";
                })
                .Build();

        private static void DisposeServices()
        {
            if(_serviceProvider == null)
                return;
            if (_serviceProvider is IDisposable)
                ((IDisposable)_serviceProvider).Dispose();
        }

    }
}