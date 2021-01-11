using System;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using Common.RabbitMQ;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RedditMonitorWorker.Logic;
using RedditMonitorWorker.ServiceConfiguration;

namespace RedditMonitorWorker
{
    public class Worker : BackgroundService
    {

        private static IRedditConsumer _worker;
        private static IContainer _container;
        private static IServiceProvider _serviceProvider;
        public IConfiguration Configuration { get; }

        public Worker(IConfiguration configuration)
        {
            Configuration = configuration;
        }


        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            BootstrapService();
            var consumeTask = Task.Factory.StartNew(() => _worker.Consume());
            await Task.WhenAll(consumeTask);
        }

        private void BootstrapService()
        {
            RegisterDependencies();
            _worker = _serviceProvider.GetService<IRedditConsumer>();
        }

        private void RegisterDependencies()
        {
            var services = new ServiceCollection();
            var configurations = new ServiceConfigurations(Configuration);
            services.AddSingleton<IRabbitConfigurations>(configurations);
            services.AddSingleton<IServiceConfigurations>(configurations);
            services.AddSingleton<IRabbitConsumer, RabbitConsumer>();
            services.AddSingleton<IStockTickerManager, StockTickerManager>();
            services.AddSingleton<IRedditConsumer, RedditConsumer>();
            services.AddSingleton<IRabbitPublisher, RabbitPublisher>();
            _serviceProvider = services.BuildServiceProvider(true);
        }

        // private static IConfiguration RegisterConfigurations()
        //     => new ConfigurationBuilder()
        //         .AddJsonFile("appsettings.json")
        //         .AddEnvironmentVariables()
        //         .AddSystemsManager(awsConfigs => {
        //             awsConfigs.AwsOptions = new Amazon.Extensions.NETCore.Setup.AWSOptions
        //             {
        //                 Profile = "socialmediadata",
        //                 Region = Amazon.RegionEndpoint.USWest2
        //             };
        //             awsConfigs.Path = "/SocialMedia";
        //         })
        //         .Build();

        private static void DisposeServices()
        {
            if(_serviceProvider == null)
                return;
            if (_serviceProvider is IDisposable)
                ((IDisposable)_serviceProvider).Dispose();
        }

    }
}